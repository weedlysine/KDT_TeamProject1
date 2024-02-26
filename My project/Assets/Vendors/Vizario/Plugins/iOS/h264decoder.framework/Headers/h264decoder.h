#pragma once

#include <h264decoderDefs.h>

//=============== C# Interface
extern "C" {
    DECLDIR bool STDCALL H264Decoder_start(const uint8_t decoderID, const VCHAR* configString);

    DECLDIR bool STDCALL H264Decoder_registerCallback(const uint8_t decoderID, void* callback);

    DECLDIR bool STDCALL H264Decoder_unregisterCallback(const uint8_t decoderID);

    DECLDIR bool STDCALL H264Decoder_commitFrame(const uint8_t decoderID, const uint8_t* rawbytes, const uint32_t rawbytessize,
      const uint32_t offset, const uint32_t length, const uint32_t pts);

    DECLDIR bool STDCALL H264Decoder_setNativeTexturePointer(const uint8_t decoderID, void* texturePointer, const VCHAR* textureInfo);

    DECLDIR void STDCALL H264Decoder_stop(const uint8_t decoderID);

    // nativeRenderEventFunc is in the cpp file...
}
