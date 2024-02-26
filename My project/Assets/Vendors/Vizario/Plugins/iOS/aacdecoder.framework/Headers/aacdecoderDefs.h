//
//  h264encoderDefs.h
//  vtcompress
//
//  Created by Clemens Arth on 05.05.21.
//

#pragma once

#if _MSC_VER >= 1900
  #include <Windows.h>
#endif

#include <stdint.h>
#include <type_traits>
#include <memory>
#include <string>

#if defined(WIN32) || defined(WINAPI_FAMILY)
#define STDCALL __cdecl
#ifdef _WINDLL
  #define DECLDIR __declspec(dllexport)
#else
  #define DECLDIR __declspec(dllimport)
#endif
#else
#define STDCALL
#define DECLDIR __attribute__ ((visibility("default")))
#endif

#include <codecvt>
#include <locale>

typedef char16_t VCHAR;

template<typename INPUTTYPE, typename OUTPUTTYPE> DECLDIR std::basic_string<OUTPUTTYPE> STDCALL convertUTFFormat8toV(const std::basic_string<INPUTTYPE>& data);
template<typename INPUTTYPE, typename OUTPUTTYPE> DECLDIR std::basic_string<OUTPUTTYPE> STDCALL convertUTFFormatVto8(const std::basic_string<INPUTTYPE>& data);
template<> DECLDIR std::basic_string<char> STDCALL convertUTFFormatVto8(const std::basic_string<char>& data);
template<> DECLDIR std::basic_string<char> STDCALL convertUTFFormatVto8(const std::basic_string<char16_t>& data);
template<> DECLDIR std::basic_string<char> STDCALL convertUTFFormat8toV(const std::basic_string<char>& data);
template<> DECLDIR std::basic_string<char16_t> STDCALL convertUTFFormat8toV(const std::basic_string<char>& data);

typedef void(STDCALL *stringCallback)(const VCHAR* message);

