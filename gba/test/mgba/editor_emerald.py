"""editor_emerald.py -- the save editor (build/editor.mb) on a real Emerald
cartridge's save, in mGBA, and the edited save loaded by Emerald itself.

    GBAGATE=/path/to/gbagate python3 -I test/mgba/editor_emerald.py \
        <emerald.gba> build/editor.mb [out-dir]

It uses gbagate's mGBA harness (test/mgba/emerald.py, its libmgba_shim and
pret's symbols in build/ref): build them there first.

  1. A new game in Emerald; eight Pokemon given by the game's own
     ScriptGiveMon (six in the party, two in box 1); Emerald writes its save
     (TrySavingData). That save, not one made here, is what the editor gets.
  2. The editor takes the GBA over, as it would after multiboot with the
     cartridge in: a monitor stops Emerald, the image goes to 0x02000000,
     and the GBA jumps to it. Then only key presses: it reads the save and
     the ROM's tables; the party's first Pokemon gets a new nickname, ten
     more levels, IVs, EVs, a move and an item; a box Pokemon gets a move;
     "Write to cartridge" writes the other slot.
  3. The flash is checked here, by a reader of this script's own (not the
     editor's C): the newer slot, the checksums, every edit.
  4. A fresh Emerald boots with that flash, CONTINUE: the game loads the
     edited save, and its party in RAM carries the edits.

No save file is kept: the flash is mGBA's, in memory. Screenshots go to
out-dir.
"""
import ctypes
import os
import struct
import sys

GBAGATE = os.environ.get('GBAGATE', '')
sys.path.insert(0, os.path.join(GBAGATE, 'test', 'mgba'))
import emerald as E      # noqa: E402
import union_room as U   # noqa: E402

FREE = E.FREE_EWRAM                      # 0x0203F000: the monitor
MBOX = FREE + 0x800
STATUS = 0x0203EF00                       # the editor's status block (editor.ld)
SIZE = 0x20000
SPECIES = [277, 280, 283, 286, 288, 290, 292, 294]   # internal: Treecko, Torchic, ... (any eight)

GIVE = '''
typedef void (*fn)(void);
typedef unsigned char (*give_fn)(unsigned short, unsigned char, unsigned short, unsigned, unsigned, unsigned char);
void entry(void) {
%s
    *(volatile unsigned *)(SYM_gMain + 4) = SYM_CB2_Overworld | 1;
}
''' % '\n'.join('    ((give_fn)(SYM_ScriptGiveMon | 1))(%d, %d, 0, 0, 0, 0);' % (sp, 5 + k)
                 for k, sp in enumerate(SPECIES))

RECALC = '''
typedef void (*calc_fn)(void *);
void entry(void) {
    ((calc_fn)(SYM_CalculateMonStats | 1))((void *)SYM_gPlayerParty);
    *(volatile unsigned *)(SYM_gMain + 4) = SYM_CB2_Overworld | 1;
}
'''

SAVE = '''
typedef unsigned char (*save_fn)(unsigned char);
void entry(void) {
    *(volatile unsigned *)RESULT = ((save_fn)(SYM_TrySavingData | 1))(0);   /* SAVE_NORMAL */
    *(volatile unsigned *)(SYM_gMain + 4) = SYM_CB2_Overworld | 1;
}
'''

# Emerald stopped for good; then a jump to the multiboot entry (ARM, 0x020000C0)
MONITOR = '''
typedef void (*fn)(void);
void entry(void) {
    *(volatile unsigned short *)0x04000208 = 0;                      /* IME */
    for (int k = 0; k < 4; k++) *(volatile unsigned *)(0x040000B8 + 12 * k) = 0;   /* DMA */
    volatile unsigned *mb = (volatile unsigned *)MBOX;
    mb[1] = 1;
    while (!mb[0]) {}
    __asm__ volatile("bx %0" :: "r"(0x020000C0));
}
'''

A, B, START, SELECT, RIGHT, LEFT, UP, DOWN, R, L = E.A, E.B, E.START, E.SELECT, E.RIGHT, E.LEFT, E.UP, E.DOWN, E.R, E.L

fails = 0


def check(what, ok):
    global fails
    print('  %s  %s' % ('ok  ' if ok else 'FAIL', what))
    fails += 0 if ok else 1


# ---- a Generation 3 save, read here independently of the editor's C ------

ORDER = ['GAEM', 'GAME', 'GEAM', 'GEMA', 'GMAE', 'GMEA', 'AGEM', 'AGME', 'AEGM', 'AEMG', 'AMGE', 'AMEG',
         'EGAM', 'EGMA', 'EAGM', 'EAMG', 'EMGA', 'EMAG', 'MGAE', 'MGEA', 'MAGE', 'MAEG', 'MEGA', 'MEAG']
SIZES = [0xF2C, 0xF80, 0xF80, 0xF80, 0xF08] + [0xF80] * 8 + [0x7D0]


def chk(b, n):
    s = sum(struct.unpack('<%dI' % (n // 4), b[:n // 4 * 4])) & 0xFFFFFFFF
    return ((s >> 16) + s) & 0xFFFF


def slots(flash):
    out = []
    for slot in range(2):
        secs, good, counters = {}, True, set()
        for k in range(14):
            s = flash[(slot * 14 + k) * 0x1000:(slot * 14 + k + 1) * 0x1000]
            sid, c, sig, cnt = struct.unpack('<HHII', s[0xFF4:0x1000])
            good &= sig == 0x08012025 and sid < 14 and c == chk(s, SIZES[sid])
            secs[sid] = s[:0xF80]
            counters.add(cnt)
        good &= len(secs) == 14 and len(counters) == 1
        out.append((good, counters.pop() if len(counters) == 1 else None, secs))
    return out


def newest(flash):
    s = slots(flash)
    k = max((i for i in range(2) if s[i][0]), key=lambda i: s[i][1])
    return k, s[k][1], b''.join(s[k][2][i] for i in range(14))


def mon(raw):
    pid, otid = struct.unpack('<II', raw[:8])
    key = pid ^ otid
    dec = b''.join(struct.pack('<I', w ^ key) for w in struct.unpack('<12I', raw[32:80]))
    sub = {c: dec[i * 12:(i + 1) * 12] for i, c in enumerate(ORDER[pid % 24])}
    g, a, e, m = sub['G'], sub['A'], sub['E'], sub['M']
    ivw = struct.unpack('<I', m[4:8])[0]
    out = dict(pid=pid, species=struct.unpack('<H', g[:2])[0], item=struct.unpack('<H', g[2:4])[0],
               exp=struct.unpack('<I', g[4:8])[0], moves=list(struct.unpack('<4H', a[:8])), pp=list(a[8:12]),
               evs=list(e[:6]), ivs=[ivw >> (5 * k) & 31 for k in range(6)], nick=raw[8:18],
               ok=struct.unpack('<H', raw[28:30])[0] == sum(struct.unpack('<24H', dec)) & 0xFFFF)
    if len(raw) >= 100:
        out['level'] = raw[84]
        out['stats'] = list(struct.unpack('<7H', raw[86:100]))   # HP, max HP, Atk, Def, Spe, SpA, SpD
    return out


def enc(text):
    o = bytearray()
    for ch in text:
        o.append(0xBB + ord(ch) - ord('A') if ch.isupper() else 0xD5 + ord(ch) - ord('a'))
    return bytes(o)


# ---- driving the editor ------------------------------------------------------

def status(e):
    magic, screen, writes, last, chip, tables = struct.unpack('<IIIiII', e.read(STATUS, 24))
    return dict(magic=magic, screen=screen, writes=writes, last=last, chip=chip, tables=tables)


def key(e, k, n=1):
    for _ in range(n):
        e.press(k, hold=3, after=5)


def savedata(e):
    buf = (ctypes.c_uint8 * SIZE)()
    n = e.lib.t_savedata_get(buf, SIZE)
    return bytes(buf)[:n]


def setup(e):
    e.lib.t_savedata_get.restype = ctypes.c_size_t
    e.lib.t_savedata_get.argtypes = (ctypes.c_void_p, ctypes.c_size_t)
    e.lib.t_savedata_put.argtypes = (ctypes.c_char_p, ctypes.c_size_t)


def main(rom, mb_path, out):
    os.makedirs(out, exist_ok=True)
    print('editor: an Emerald save edited on the GBA, then loaded by Emerald')
    e = E.Emu(rom)
    setup(e)
    U.new_game(e)
    check('a new game reaches the overworld', e.name_of(e.callback2()) == 'CB2_Overworld')
    e.call_once(GIVE)
    e.run(30)
    check('the game gave eight Pokemon (party %d)' % e.r8(e.sym['gPlayerPartyCount']),
          e.r8(e.sym['gPlayerPartyCount']) == 6)
    res = FREE + 0x700
    e.w32(res, 0xFF)
    e.call_once(SAVE, defines={'RESULT': hex(res)})
    for _ in range(60):                          # mGBA times flash erases: about 3 s of game time
        e.run(15)
        if e.r32(res) != 0xFF:
            break
    check('Emerald wrote its save (TrySavingData: %d)' % e.r32(res), e.r32(res) == 1)
    before = savedata(e)
    slot0, count0, data0 = newest(before)
    party0 = [mon(data0[0xF80 + 0x238 + 100 * k:0xF80 + 0x238 + 100 * (k + 1)]) for k in range(6)]
    check('its party: the six given first (%s)' % [p['species'] for p in party0],
          [p['species'] for p in party0] == SPECIES[:6])
    box0 = [mon(data0[5 * 0xF80 + 4 + 80 * k:5 * 0xF80 + 4 + 80 * (k + 1)]) for k in range(2)]
    check('box 1: the other two (%s)' % [b['species'] for b in box0], [b['species'] for b in box0] == SPECIES[6:])

    # the editor takes over
    e.w32(MBOX, 0)
    e.w32(MBOX + 4, 0)
    e.call_once(MONITOR, defines={'MBOX': hex(MBOX)})
    for _ in range(10):
        e.run(1)
        if e.r32(MBOX + 4):
            break
    check('Emerald stopped by the monitor', e.r32(MBOX + 4) == 1)
    e.write(0x02000000, open(mb_path, 'rb').read())
    e.w32(STATUS, 0)
    e.w32(MBOX, 1)
    for _ in range(600):
        e.run(10)
        if status(e)['magic'] == 0x42474B50:
            break
    st = status(e)
    check('the editor loaded the save (flash %04X, %d ROM tables)' % (st['chip'], st['tables']),
          st['magic'] == 0x42474B50 and st['tables'] == 5 and st['chip'] == 0x1362)
    e.run(5)
    e.shot(os.path.join(out, '1-main.png'))

    key(e, A)                                    # Party
    check('the party screen', status(e)['screen'] == 1)
    e.shot(os.path.join(out, '2-party.png'))
    key(e, A)                                    # the first Pokemon
    check('the Pokemon screen', status(e)['screen'] == 3)
    e.shot(os.path.join(out, '3-before.png'))
    # nickname: A opens the editor; 'Z' at position 0 is one Down from 'A' (it wraps)
    key(e, A)
    check('the name editor', status(e)['screen'] == 4)
    key(e, SELECT)                               # end at 0: an empty name
    key(e, DOWN)                                 # an empty place goes to the list's end ('/')
    key(e, UP)                                   # ... and one on wraps to 'A'
    key(e, RIGHT)
    key(e, UP, 4)                                # 'D'
    key(e, A)
    key(e, DOWN)                                 # Level
    key(e, R)                                    # +10
    key(e, DOWN)                                 # Item
    key(e, RIGHT, 13)                            # item 13 (Potion, ...; the ROM's name)
    key(e, DOWN)                                 # Move 1
    key(e, LEFT)                                 # one less: Pound (1) -> 0, an empty slot
    for _ in range(3):
        key(e, DOWN)                             # to Move 4
    key(e, RIGHT)
    key(e, DOWN)                                 # IVs: HP
    key(e, R, 4)                                 # +40: held at 31
    key(e, A)                                    # Atk
    key(e, LEFT, 3)
    key(e, DOWN)                                 # EVs: the column stays on Atk
    key(e, R, 26)                                # +260: held at 255
    key(e, A)                                    # Def
    key(e, R, 26)                                # 255 more: 510 in all
    key(e, A, 4)                                 # SpD
    key(e, R)                                    # nothing more fits
    e.shot(os.path.join(out, '4-edited.png'))
    key(e, B)                                    # back to the party
    key(e, B)                                    # the menu
    key(e, DOWN)                                 # Boxes
    key(e, A)
    check('the box screen', status(e)['screen'] == 2)
    e.shot(os.path.join(out, '5-box.png'))
    key(e, RIGHT)                                # the second Pokemon in box 1
    key(e, A)
    for _ in range(3):
        key(e, DOWN)                             # Move 1
    key(e, RIGHT, 2)
    key(e, B)
    key(e, B)
    key(e, DOWN)                                 # Write to cartridge
    key(e, A)
    check('asked to confirm', status(e)['screen'] == 5)
    key(e, A)
    for _ in range(600):
        e.run(5)
        if status(e)['writes'] or status(e)['last']:
            break
    e.run(5)
    e.shot(os.path.join(out, '6-written.png'))
    st = status(e)
    check('written and verified on the GBA (writes %d, result %d)' % (st['writes'], st['last']),
          st['writes'] == 1 and st['last'] == 0)

    after = savedata(e)
    slot1, count1, data1 = newest(after)
    check('the flash: the other slot (%d -> %d), counter + 1 (%d -> %d), both slots whole' % (
        slot0, slot1, count0, count1), slot1 == slot0 ^ 1 and count1 == count0 + 1 and all(s[0] for s in slots(after)))
    check('... the old slot left as it was',
          after[slot0 * 0xE000:(slot0 + 1) * 0xE000] == before[slot0 * 0xE000:(slot0 + 1) * 0xE000])
    check('... sectors 28..31 left as they were', after[0x1C000:] == before[0x1C000:])
    p0, p1 = party0[0], mon(data1[0xF80 + 0x238:0xF80 + 0x238 + 100])
    print('    before: %s' % {k: p0[k] for k in ('species', 'level', 'exp', 'item', 'moves', 'ivs', 'evs', 'stats')})
    print('    after:  %s' % {k: p1[k] for k in ('species', 'level', 'exp', 'item', 'moves', 'ivs', 'evs', 'stats')})
    check('the record\'s checksum is good', p1['ok'])
    check('nickname AD', p1['nick'][:3] == enc('AD') + b'\xff')
    check('level %d -> %d' % (p0['level'], p1['level']), p1['level'] == p0['level'] + 10)
    check('item 13', p1['item'] == 13)
    check('moves 1 and 4 changed (%s -> %s)' % (p0['moves'], p1['moves']),
          p1['moves'][0] == p0['moves'][0] - 1 and p1['moves'][3] == p0['moves'][3] + 1)
    check('IVs: HP 31, Atk 3 less', p1['ivs'][0] == 31 and p1['ivs'][1] == max(0, p0['ivs'][1] - 3))
    check('EVs: Atk 255, Def 255, then no more (510 in all)', p1['evs'] == [0, 255, 255, 0, 0, 0])
    check('stats higher for the levels and EVs', all(b > a for a, b in zip(p0['stats'][1:], p1['stats'][1:])))
    others = all(data1[0xF80 + 0x238 + 100 * k:0xF80 + 0x238 + 100 * (k + 1)] ==
                 data0[0xF80 + 0x238 + 100 * k:0xF80 + 0x238 + 100 * (k + 1)] for k in range(1, 6))
    check('the other five party records untouched', others)
    b1 = mon(data1[5 * 0xF80 + 4 + 80:5 * 0xF80 + 4 + 160])
    check('box 1, second Pokemon: move 1 two on (%d -> %d)' % (box0[1]['moves'][0], b1['moves'][0]),
          b1['ok'] and b1['moves'][0] == box0[1]['moves'][0] + 2)

    # Emerald loads it
    g = E.Emu(rom)
    setup(g)
    check('a fresh Emerald gets the edited flash', g.lib.t_savedata_put(after, len(after)) == 0)
    for i in range(1500):
        if g.name_of(g.callback2()) == 'CB2_Overworld':
            break
        g.press(A if i % 3 else START, hold=3, after=9)
    g.run(120)
    check('CONTINUE reaches the overworld (%s)' % g.name_of(g.callback2()), g.name_of(g.callback2()) == 'CB2_Overworld')
    g.shot(os.path.join(out, '7-emerald.png'))
    lead = mon(g.read(g.sym['gPlayerParty'], 100))
    check('Emerald\'s party in RAM: the edited lead, level %d, nickname, item, IVs, EVs' % lead['level'],
          lead['ok'] and lead['level'] == p1['level'] and lead['nick'] == p1['nick'] and lead['item'] == 13 and
          lead['ivs'] == p1['ivs'] and lead['evs'] == p1['evs'] and lead['moves'] == p1['moves'])
    g.call_once(RECALC)
    g.run(5)
    game = mon(g.read(g.sym['gPlayerParty'], 100))
    check('Emerald\'s own CalculateMonStats gives the editor\'s stats (%s)' % game['stats'],
          game['stats'] == p1['stats'] and game['level'] == p1['level'])
    print('editor: %s' % ('all checks passed' if not fails else '%d FAILED' % fails))
    return 1 if fails else 0


if __name__ == '__main__':
    sys.exit(main(sys.argv[1], sys.argv[2], sys.argv[3] if len(sys.argv) > 3 else 'build/editor-shots'))
