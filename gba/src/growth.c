/* growth.c -- see growth.h. */
#include "growth.h"

static int32_t cube(int32_t n) { return n * n * n; }

uint32_t growth_exp(int rate, int level) {
    const int32_t n = level;
    if (level <= 0) return 0;
    if (level == 1) return 1;      /* the tables' level 1 entry, at every rate */
    int32_t v;
    switch (rate) {
    case GROWTH_ERRATIC:
        if (n <= 50) v = cube(n) * (100 - n) / 50;
        else if (n <= 68) v = cube(n) * (150 - n) / 100;
        else if (n <= 98) v = cube(n) * ((1911 - 10 * n) / 3) / 500;
        else v = cube(n) * (160 - n) / 100;
        break;
    case GROWTH_FLUCTUATING:
        if (n <= 15) v = cube(n) * ((n + 1) / 3 + 24) / 50;
        else if (n <= 36) v = cube(n) * (n + 14) / 50;
        else v = cube(n) * (n / 2 + 32) / 50;
        break;
    case GROWTH_MEDIUM_SLOW: v = 6 * cube(n) / 5 - 15 * n * n + 100 * n - 140; break;
    case GROWTH_FAST: v = 4 * cube(n) / 5; break;
    case GROWTH_SLOW: v = 5 * cube(n) / 4; break;
    default: v = cube(n); break;
    }
    return (uint32_t)v;
}

int growth_level(int rate, uint32_t exp) {
    int level = 1;
    while (level < 100 && growth_exp(rate, level + 1) <= exp) level++;
    return level;
}

int growth_nature_effect(int nature, int stat) {
    if (stat < 1 || stat > 5) return 0;
    const int up = nature / 5 + 1, down = nature % 5 + 1;
    if (up == down) return 0;
    return stat == up ? 1 : stat == down ? -1 : 0;
}

void growth_stats(const species_stats_t *sp, uint16_t species, int level, int nature,
                  const uint8_t iv[6], const uint8_t ev[6], uint16_t out[6]) {
    if (species == SPECIES_SHEDINJA) out[0] = 1;
    else out[0] = (uint16_t)((2 * sp->base[0] + iv[0] + ev[0] / 4) * level / 100 + level + 10);
    for (int s = 1; s < 6; s++) {
        int n = (2 * sp->base[s] + iv[s] + ev[s] / 4) * level / 100 + 5;
        const int e = growth_nature_effect(nature, s);
        if (e > 0) n = n * 110 / 100;
        if (e < 0) n = n * 90 / 100;
        out[s] = (uint16_t)n;
    }
}

uint16_t growth_new_hp(uint16_t species, uint16_t hp, uint16_t old_max, uint16_t new_max) {
    if (species == SPECIES_SHEDINJA) return (hp != 0 || old_max == 0) ? 1 : hp;
    if (hp == 0 && old_max == 0) return new_max;
    if (hp == 0) return 0;
    const int v = (int)hp + (int)new_max - (int)old_max;
    return (uint16_t)(v <= 0 ? 1 : v);
}

const char *const nature_names[25] = {
    "Hardy", "Lonely", "Brave", "Adamant", "Naughty", "Bold", "Docile", "Relaxed",
    "Impish", "Lax", "Timid", "Hasty", "Serious", "Jolly", "Naive", "Modest", "Mild",
    "Quiet", "Bashful", "Rash", "Calm", "Gentle", "Sassy", "Careful", "Quirky",
};
