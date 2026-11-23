/* flash.c -- GBA cartridge flash, from code running in EWRAM (as code that
 * sends flash commands must). Adapted from gbagate's rom/savetool.c
 * (GPL-3.0), which is tested against mGBA's emulated chip; see there for
 * the chips and their IDs. */
#include "flash.h"

#define SAVE        ((volatile uint8_t *)0x0E000000)
#define REG_WAITCNT (*(volatile uint16_t *)0x04000204)
#define REG_IME     (*(volatile uint16_t *)0x04000208)

static uint32_t s_id;

static void cmd(uint8_t c) {
    SAVE[0x5555] = 0xAA;
    SAVE[0x2AAA] = 0x55;
    SAVE[0x5555] = c;
}
static void bank(uint32_t b) { cmd(0xB0); SAVE[0] = (uint8_t)b; }
static int wait_for(volatile uint8_t *a, uint8_t want, uint32_t spins) {
    while (spins--) if (*a == want) return 1;
    return 0;
}

uint32_t flash_chip_id(void) {
    REG_WAITCNT = 0x4317;          /* SRAM/flash: 8 wait states */
    cmd(0x90);
    for (volatile int i = 0; i < 2000; i++) {}
    s_id = (uint32_t)SAVE[0] | (uint32_t)SAVE[1] << 8;
    cmd(0xF0);
    cmd(0xF0);
    for (volatile int i = 0; i < 2000; i++) {}
    return s_id;
}

uint32_t flash_chip_size(uint32_t id) {
    switch (id) {
    case 0x1362: case 0x09C2: return 0x20000;                       /* Sanyo, Macronix 1M */
    case 0x3D1F: case 0x1B32: case 0xD4BF: case 0x1CC2: return 0x10000;
    default: return 0;
    }
}

int flash_read(void *ctx, unsigned sector, uint8_t *buf) {
    (void)ctx;
    if (sector >= 32) return -1;
    const uint32_t off = sector * 0x1000u;
    bank(off >> 16);
    for (uint32_t k = 0; k < 0x1000; k++) buf[k] = SAVE[(off & 0xFFFF) + k];
    bank(0);
    return 0;
}

int flash_write(void *ctx, unsigned sector, const uint8_t *src) {
    (void)ctx;
    if (sector >= 32) return -1;
    const uint32_t off = sector * 0x1000u;
    bank(off >> 16);
    volatile uint8_t *dst = &SAVE[off & 0xFFFF];
    const uint16_t ime = REG_IME;
    REG_IME = 0;
    int ok = 1;
    if ((s_id & 0xFF) == 0x1F) {                       /* Atmel: 128-byte pages */
        for (uint32_t p = 0; p < 0x1000 / 128 && ok; p++) {
            cmd(0xA0);
            for (uint32_t k = 0; k < 128; k++) dst[p * 128 + k] = src[p * 128 + k];
            ok = wait_for(&dst[p * 128 + 127], src[p * 128 + 127], 400000);
        }
    } else {                                           /* erase the sector, then bytes */
        cmd(0x80);
        SAVE[0x5555] = 0xAA;
        SAVE[0x2AAA] = 0x55;
        *dst = 0x30;
        ok = wait_for(dst, 0xFF, 2000000);
        for (uint32_t k = 0; k < 0x1000 && ok; k++) {
            if (src[k] == 0xFF) continue;
            cmd(0xA0);
            dst[k] = src[k];
            ok = wait_for(&dst[k], src[k], 20000);
        }
    }
    REG_IME = ime;
    bank(0);
    return ok ? 0 : -1;
}
