/* g3save.h -- a Generation 3 save: two slots of 14 sectors, the newer one
 * read, edits written to the other one as the game itself saves.
 *
 * Sector layout (4 KB): data in the first 0xF80 bytes; at 0xFF4 the section
 * id (u16), 0xFF6 its checksum (u16), 0xFF8 the signature 0x08012025,
 * 0xFFC the save counter (u32). Sections 0..13: 0 SaveBlock2 (the trainer),
 * 1..4 SaveBlock1 (the party among them), 5..13 the PC's boxes. The game
 * rotates which sector holds which section on every save, so sections are
 * found by id. Sectors 28..31 (Hall of Fame, Trainer Hill, recorded battle)
 * are left alone.
 *
 * Nothing here touches hardware: the caller passes sector read and write
 * functions (a cartridge's flash on the GBA, a buffer in the tests).
 */
#ifndef G3SAVE_H
#define G3SAVE_H
#include <stdint.h>

#define G3_SECTOR       0x1000
#define G3_SECTOR_DATA  0xF80
#define G3_SECTIONS     14
#define G3_SIGNATURE    0x08012025u
#define G3_DATA_SIZE    (G3_SECTIONS * G3_SECTOR_DATA)   /* 55552 */

enum g3_game { G3_GAME_UNKNOWN, G3_GAME_RS, G3_GAME_E, G3_GAME_FRLG };

typedef int (*g3_read_fn)(void *ctx, unsigned sector, uint8_t *buf4k);
typedef int (*g3_write_fn)(void *ctx, unsigned sector, const uint8_t *buf4k);

typedef struct {
    int game;
    int slot;                      /* the slot read: 0 or 1 */
    uint32_t counter;              /* its save counter */
    uint8_t rot;                   /* the sector, within the slot, of section 0 */
    int valid[2];                  /* each slot's 14 sections present and checksummed */
    uint32_t counters[2];
    uint8_t *data;                 /* G3_DATA_SIZE bytes: section k at k * 0xF80 */
} g3_save_t;

enum {
    G3_OK = 0,
    G3_ERR_IO = -1,                /* the reader or writer failed */
    G3_ERR_NO_SAVE = -2,           /* neither slot is whole */
    G3_ERR_VERIFY = -3,            /* written, but did not read back */
};

/* The game, from a cartridge's four-letter code (AXVE, BPEE, BPRE, ...). */
int g3_game_from_code(const char code[4]);
/* The bytes the game checksums in section id. */
uint32_t g3_section_size(int game, int id);
/* The game's checksum of n bytes: 32-bit word sum, halves folded. */
uint16_t g3_checksum(const uint8_t *p, uint32_t n);

/* Read both slots' footers and the newer whole slot's sections into
 * s->data, using tmp (4 KB) for each sector. s->game must be set. */
int g3_load(g3_save_t *s, g3_read_fn rd, void *ctx, uint8_t *tmp);

/* Write s->data to the other slot with the counter one higher, and read
 * every sector back. On success s describes the new slot. */
int g3_commit(g3_save_t *s, g3_read_fn rd, g3_write_fn wr, void *ctx, uint8_t *tmp);

/* ---- what is in the sections -------------------------------------------- */
#define G3_BOXES        14
#define G3_BOX_SLOTS    30
#define G3_MON_BOX      80
#define G3_MON_PARTY    100

uint8_t *g3_sb2(g3_save_t *s);                 /* section 0 */
uint8_t *g3_sb1(g3_save_t *s);                 /* sections 1..4, contiguous */
uint8_t *g3_storage(g3_save_t *s);             /* sections 5..13, contiguous */
int g3_party_count(g3_save_t *s);
uint8_t *g3_party_mon(g3_save_t *s, int k);    /* 100 bytes */
uint8_t *g3_box_mon(g3_save_t *s, int box, int k);   /* 80 bytes */
uint8_t *g3_box_name(g3_save_t *s, int box);   /* 9 game characters */
int g3_current_box(g3_save_t *s);
#endif
