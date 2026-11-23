/* mem.h -- the few memory helpers the editor needs, with no C library on
 * the GBA (mem.c) and the C library's in the host tests. */
#ifndef MEM_H
#define MEM_H
#include <stdint.h>
#include <stddef.h>
void mem_copy(void *dst, const void *src, size_t n);
void mem_set(void *dst, int v, size_t n);
int mem_equal(const void *a, const void *b, size_t n);
#endif
