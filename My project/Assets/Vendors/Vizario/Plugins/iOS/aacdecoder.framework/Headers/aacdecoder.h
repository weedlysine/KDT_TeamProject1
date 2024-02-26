#pragma once

#include <aacdecoderDefs.h>

//=============== C# Interface
extern "C" {
    DECLDIR bool STDCALL AACDecoder_start(const uint8_t decoderID);

    DECLDIR bool STDCALL AACDecoder_registerCallback(const uint8_t decoderID, void* callback);

    DECLDIR bool STDCALL AACDecoder_unregisterCallback(const uint8_t decoderID);

    DECLDIR bool STDCALL AACDecoder_commitFrame(const uint8_t decoderID, const uint8_t* rawbytes, const uint32_t rawbytessize,
      const uint32_t offset, const uint32_t length, const uint32_t pts);

    DECLDIR void STDCALL AACDecoder_stop(const uint8_t decoderID);

    // nativeRenderEventFunc is in the cpp file...
}
