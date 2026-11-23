/* mon.h -- one Pokemon record (pk3): 80 bytes in a box, 100 in the party.
 *
 * Bytes 0..31 are plain: personality (PID), OT ID, nickname (10), language,
 * flags (bit 0 bad egg, 1 has species, 2 is egg), OT name (7), markings,
 * checksum (u16 at 28). Bytes 32..79 are four 12-byte substructures --
 * Growth, Attacks, EVs, Misc -- in an order set by PID % 24, each u32 XORed
 * with PID ^ OTID; the checksum is the u16 sum of them decrypted. A party
 * record adds 20 plain bytes: status, level, then HP and the six stats.
 *
 * A mon_t holds the record with its substructures decrypted and put in
 * G, A, E, M order, so the accessors below need no table.
 */
#ifndef MON_H
#define MON_H
#include <stdint.h>

typedef struct {
    uint8_t b[100];          /* 0..31 plain, 32..79 G A E M decrypted, 80..99 party */
    int party;
    int checksum_ok;
} mon_t;

enum { STAT_HP, STAT_ATK, STAT_DEF, STAT_SPE, STAT_SPA, STAT_SPD };   /* the games' EV/IV order */

void mon_load(mon_t *m, const uint8_t *src, int party);
void mon_store(const mon_t *m, uint8_t *dst);       /* encrypts, sets the checksum */
uint16_t mon_checksum(const mon_t *m);

uint32_t mon_pid(const mon_t *m);
uint32_t mon_otid(const mon_t *m);
int mon_present(const mon_t *m);                     /* a species is there */
int mon_is_egg(const mon_t *m);
uint16_t mon_species(const mon_t *m);
uint16_t mon_item(const mon_t *m);
void mon_set_item(mon_t *m, uint16_t v);
uint32_t mon_exp(const mon_t *m);
void mon_set_exp(mon_t *m, uint32_t v);
uint8_t mon_pp_bonuses(const mon_t *m);
uint16_t mon_move(const mon_t *m, int k);
uint8_t mon_pp(const mon_t *m, int k);
void mon_set_move(mon_t *m, int k, uint16_t move, uint8_t pp);
uint8_t mon_ev(const mon_t *m, int stat);
void mon_set_ev(mon_t *m, int stat, uint8_t v);
int mon_ev_total(const mon_t *m);
uint8_t mon_iv(const mon_t *m, int stat);
void mon_set_iv(mon_t *m, int stat, uint8_t v);
int mon_ability_bit(const mon_t *m);
int mon_nature(const mon_t *m);                      /* PID % 25 */
uint8_t *mon_nickname(mon_t *m);                     /* 10 game characters */
uint8_t *mon_ot_name(mon_t *m);                      /* 7 */

/* Party only. */
uint8_t mon_level(const mon_t *m);
uint16_t mon_stat(const mon_t *m, int stat);         /* STAT_HP: the maximum */
uint16_t mon_hp(const mon_t *m);
void mon_set_party_stats(mon_t *m, uint8_t level, const uint16_t stats[6], uint16_t hp);
#endif
