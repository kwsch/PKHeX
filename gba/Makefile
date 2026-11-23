# gba/Makefile -- the Generation 3 save editor, as a GBA multiboot program.
#
#   make test                        the portable core, on the host
#   make test ROM=x.gba SYM=x.sym    also against a ROM and pret's symbols
#   make                             editor.mb (arm-none-eabi-gcc)
#   make font                        src/font_data.c from tools/font.txt
CORE := src/g3save.c src/mon.c src/g3text.c src/growth.c src/romdata.c src/edit.c src/mem.c
HOSTCC ?= cc

.PHONY: all test font clean mgba
all: build/editor.mb

CROSS ?= arm-none-eabi-
GBA_CFLAGS := -mcpu=arm7tdmi -mthumb -mthumb-interwork -O2 -ffreestanding -fno-builtin \
              -std=c11 -Wall -Wextra -ffunction-sections -fdata-sections
GBA_SRC := $(CORE) gba/main.c gba/hw.c gba/flash.c gba/libc_min.c src/font_data.c

build/editor.elf: gba/crt0.s gba/editor.ld $(GBA_SRC) src/*.h gba/*.h | build
	$(CROSS)gcc $(GBA_CFLAGS) -nostdlib -nostartfiles -T gba/editor.ld -Wl,--gc-sections \
	    -Wl,-Map,build/editor.map -o $@ gba/crt0.s $(GBA_SRC) -lgcc

# The image: everything up to the footer (.bss and the status block are not sent)
build/editor.mb: build/editor.elf
	$(CROSS)objcopy -O binary -j .text -j .data -j .mbfoot $< $@
	@size=$$(stat -c %s $@); echo "editor.mb: $$size bytes"; \
	    test $$size -le 262144 || { echo "too big for multiboot"; exit 1; }

build:
	mkdir -p build

build/test_core: test/test_core.c $(CORE) src/*.h | build
	$(HOSTCC) -std=gnu11 -O1 -g -Wall -Wextra -fsanitize=address,undefined -o $@ test/test_core.c $(CORE)

test: build/test_core
	./build/test_core $(if $(ROM),"$(ROM)" "$(SYM)")

font:
	python3 tools/mkfont.py tools/font.txt src/font_data.c

clean:
	rm -rf build

# The editor on a real Emerald save in mGBA, then Emerald loading the result.
# Needs gbagate's mGBA harness built (GBAGATE=path) and a ROM you own.
mgba: build/editor.mb
	GBAGATE=$(GBAGATE) python3 -I test/mgba/editor_emerald.py "$(ROM)" build/editor.mb build/editor-shots
