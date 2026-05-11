
In FSNodeLIBRETRO.cxx
`size_t FSNodeLIBRETRO::read(ByteArray& image, size_t) const
{
  // BUGFIX: original code resized image to Cartridge::maxSize() (524288),
  // then libretro_read_rom only wrote getROMSize() bytes — leaving the
  // remainder as whatever the vector contained before (uninitialised on
  // first load, stale ROM tail on subsequent loads). Downstream code
  // examines image.size() to pick a bankswitch scheme, so a 2KB Combat
  // cart looked like a 524KB cart, got truncated to 4KB, and the 6502
  // ran headlong into the garbage half. Resize to the actual rom size so
  // image.size() reflects reality.
  extern uInt32 libretro_get_rom_size(void);
  image.resize(libretro_get_rom_size());

  extern uInt32 libretro_read_rom(void* data);
  return libretro_read_rom(image.data());
}`