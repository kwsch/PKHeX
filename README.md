PKHeX
=====

Pokémon X/Y SAV/PKX file editor, programmed in C#.
Supports binary file loading of:

	Decrypted Save files (1MB/Cyber Save 0x65600 Bytes) 
	Individual entity files (232/260 bytes)
	Obsoleted: Unencrypted Network Packets containing entity files.
	
Data is then displayed in a meaningful view, and can be edited and saved back to Binary data.
For loaded save files, saving an edited copy is possible and all hashes will be fixed.
Since save data uses an AES MAC that cannot be emulated without the 3DS's keys, a resigning service is required.

The interface is translatable with resource/external text files so that different languages can be supported.

Note: I started learning C# by making this program, so certain implementations may look odd ;)
