/* growth.h -- experience for a level and the stats a level gives, as the
 * games compute them (pret's experience_tables.c and CalculateMonStats). */
#ifndef GROWTH_H
#define GROWTH_H
#include <stdint.h>

enum { GROWTH_MEDIUM_FAST, GROWTH_ERRATIC, GROWTH_FLUCTUATING, GROWTH_MEDIUM_SLOW,
       GROWTH_FAST, GROWTH_SLOW };

#define SPECIES_SHEDINJA 303      /* internal index: max HP is always 1 */

/* The least experience for level (0..100) at a growth rate. */
uint32_t growth_exp(int rate, int level);
/* The level that experience reaches (1..100). */
int growth_level(int rate, uint32_t exp);

/* A species' base stats, in the games' order (HP, Atk, Def, Speed, SpA,
 * SpD), and its growth rate. */
typedef struct { uint8_t base[6]; uint8_t growth; } species_stats_t;

/* Stats for a level: out[0] max HP, then Atk, Def, Speed, SpA, SpD. */
void growth_stats(const species_stats_t *sp, uint16_t species, int level, int nature,
                  const uint8_t iv[6], const uint8_t ev[6], uint16_t out[6]);
/* The HP the game leaves a Pokemon with when its max HP changes. */
uint16_t growth_new_hp(uint16_t species, uint16_t hp, uint16_t old_max, uint16_t new_max);
/* +1, -1 or 0: how nature changes stat (1..5). */
int growth_nature_effect(int nature, int stat);
extern const char *const nature_names[25];
#endif
