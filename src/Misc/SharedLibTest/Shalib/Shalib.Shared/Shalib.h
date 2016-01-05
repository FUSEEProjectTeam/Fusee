#pragma once

class Shalib {
public:
    static char * getTemplateInfo();
    Shalib();
    ~Shalib();
};


extern "C" int NativeAdd(int a, int b);
