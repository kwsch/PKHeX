/* editor.ld -- the editor as one multiboot image in EWRAM. Adapted from
 * gbagate's rom/multiboot.ld (GPL-3.0). */
OUTPUT_FORMAT("elf32-littlearm")
OUTPUT_ARCH(arm)
ENTRY(_start)

MEMORY {
    ewram (rwx) : ORIGIN = 0x02000000, LENGTH = 256K
    iwram (rwx) : ORIGIN = 0x03000000, LENGTH = 32K
}
__stack_top = ORIGIN(iwram) + LENGTH(iwram) - 0x100;

SECTIONS {
    .text : {
        KEEP(*(.gba_crt0))
        *(.text .text.*)
        *(.rodata .rodata.*)
        . = ALIGN(4);
    } > ewram
    .data : {
        . = ALIGN(4);
        __data_start__ = .;
        *(.data .data.*)
        . = ALIGN(4);
    } > ewram
    __data_lma__ = LOADADDR(.data);
    .mbfoot : {
        . = ALIGN(4);
        KEEP(*(.mbfoot))
        __image_end__ = .;
    } > ewram
    .bss (NOLOAD) : {
        . = ALIGN(4);
        __bss_start__ = .;
        *(.bss .bss.*)
        *(COMMON)
        . = ALIGN(4);
        __bss_end__ = .;
    } > ewram
    /* the editor's status, for tests: fixed, and outside the image */
    .status 0x0203EF00 (NOLOAD) : { KEEP(*(.status)) } > ewram
    /DISCARD/ : { *(.comment) *(.note.*) *(.ARM.attributes) *(.ARM.exidx*) *(.ARM.extab*) }
}
ASSERT(__data_lma__ == __data_start__, ".data must load where it runs")
/* the mGBA test's monitor and mailbox sit at 0x0203F000 */
ASSERT(__bss_end__ <= 0x0203EF00, "the editor reaches its status block at 0x0203EF00")
