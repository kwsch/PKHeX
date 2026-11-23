/* flash.h -- a cartridge's 128 KB flash save, a 4 KB sector at a time. */
#ifndef FLASH_H
#define FLASH_H
#include <stdint.h>
uint32_t flash_chip_id(void);            /* maker | device << 8 */
uint32_t flash_chip_size(uint32_t id);   /* 0x20000, 0x10000, or 0 unknown */
int flash_read(void *ctx, unsigned sector, uint8_t *buf);
int flash_write(void *ctx, unsigned sector, const uint8_t *buf);
#endif
