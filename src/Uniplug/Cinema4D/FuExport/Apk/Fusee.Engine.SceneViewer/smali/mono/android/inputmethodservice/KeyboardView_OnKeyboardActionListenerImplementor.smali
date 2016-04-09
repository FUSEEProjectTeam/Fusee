.class public Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;
.super Ljava/lang/Object;
.source "KeyboardView_OnKeyboardActionListenerImplementor.java"

# interfaces
.implements Lmono/android/IGCUserPeer;
.implements Landroid/inputmethodservice/KeyboardView$OnKeyboardActionListener;


# static fields
.field static final __md_methods:Ljava/lang/String;


# instance fields
.field refList:Ljava/util/ArrayList;


# direct methods
.method static constructor <clinit>()V
    .locals 3

    .prologue
    .line 12
    const-string v0, "n_onKey:(I[I)V:GetOnKey_IarrayIHandler:Android.InputMethodServices.KeyboardView/IOnKeyboardActionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onPress:(I)V:GetOnPress_IHandler:Android.InputMethodServices.KeyboardView/IOnKeyboardActionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onRelease:(I)V:GetOnRelease_IHandler:Android.InputMethodServices.KeyboardView/IOnKeyboardActionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onText:(Ljava/lang/CharSequence;)V:GetOnText_Ljava_lang_CharSequence_Handler:Android.InputMethodServices.KeyboardView/IOnKeyboardActionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_swipeDown:()V:GetSwipeDownHandler:Android.InputMethodServices.KeyboardView/IOnKeyboardActionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_swipeLeft:()V:GetSwipeLeftHandler:Android.InputMethodServices.KeyboardView/IOnKeyboardActionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_swipeRight:()V:GetSwipeRightHandler:Android.InputMethodServices.KeyboardView/IOnKeyboardActionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_swipeUp:()V:GetSwipeUpHandler:Android.InputMethodServices.KeyboardView/IOnKeyboardActionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n"

    sput-object v0, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;->__md_methods:Ljava/lang/String;

    .line 22
    const-string v0, "Android.InputMethodServices.KeyboardView+IOnKeyboardActionListenerImplementor, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-class v1, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;

    sget-object v2, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;->__md_methods:Ljava/lang/String;

    invoke-static {v0, v1, v2}, Lmono/android/Runtime;->register(Ljava/lang/String;Ljava/lang/Class;Ljava/lang/String;)V

    .line 23
    return-void
.end method

.method public constructor <init>()V
    .locals 3
    .annotation system Ldalvik/annotation/Throws;
        value = {
            Ljava/lang/Throwable;
        }
    .end annotation

    .prologue
    .line 28
    invoke-direct {p0}, Ljava/lang/Object;-><init>()V

    .line 29
    invoke-virtual {p0}, Ljava/lang/Object;->getClass()Ljava/lang/Class;

    move-result-object v0

    const-class v1, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;

    if-ne v0, v1, :cond_0

    .line 30
    const-string v0, "Android.InputMethodServices.KeyboardView+IOnKeyboardActionListenerImplementor, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-string v1, ""

    const/4 v2, 0x0

    new-array v2, v2, [Ljava/lang/Object;

    invoke-static {v0, v1, p0, v2}, Lmono/android/TypeManager;->Activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V

    .line 31
    :cond_0
    return-void
.end method

.method private native n_onKey(I[I)V
.end method

.method private native n_onPress(I)V
.end method

.method private native n_onRelease(I)V
.end method

.method private native n_onText(Ljava/lang/CharSequence;)V
.end method

.method private native n_swipeDown()V
.end method

.method private native n_swipeLeft()V
.end method

.method private native n_swipeRight()V
.end method

.method private native n_swipeUp()V
.end method


# virtual methods
.method public monodroidAddReference(Ljava/lang/Object;)V
    .locals 1

    .prologue
    .line 100
    iget-object v0, p0, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;->refList:Ljava/util/ArrayList;

    if-nez v0, :cond_0

    .line 101
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    iput-object v0, p0, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;->refList:Ljava/util/ArrayList;

    .line 102
    :cond_0
    iget-object v0, p0, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0, p1}, Ljava/util/ArrayList;->add(Ljava/lang/Object;)Z

    .line 103
    return-void
.end method

.method public monodroidClearReferences()V
    .locals 1

    .prologue
    .line 107
    iget-object v0, p0, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;->refList:Ljava/util/ArrayList;

    if-eqz v0, :cond_0

    .line 108
    iget-object v0, p0, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0}, Ljava/util/ArrayList;->clear()V

    .line 109
    :cond_0
    return-void
.end method

.method public onKey(I[I)V
    .locals 0

    .prologue
    .line 36
    invoke-direct {p0, p1, p2}, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;->n_onKey(I[I)V

    .line 37
    return-void
.end method

.method public onPress(I)V
    .locals 0

    .prologue
    .line 44
    invoke-direct {p0, p1}, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;->n_onPress(I)V

    .line 45
    return-void
.end method

.method public onRelease(I)V
    .locals 0

    .prologue
    .line 52
    invoke-direct {p0, p1}, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;->n_onRelease(I)V

    .line 53
    return-void
.end method

.method public onText(Ljava/lang/CharSequence;)V
    .locals 0

    .prologue
    .line 60
    invoke-direct {p0, p1}, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;->n_onText(Ljava/lang/CharSequence;)V

    .line 61
    return-void
.end method

.method public swipeDown()V
    .locals 0

    .prologue
    .line 68
    invoke-direct {p0}, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;->n_swipeDown()V

    .line 69
    return-void
.end method

.method public swipeLeft()V
    .locals 0

    .prologue
    .line 76
    invoke-direct {p0}, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;->n_swipeLeft()V

    .line 77
    return-void
.end method

.method public swipeRight()V
    .locals 0

    .prologue
    .line 84
    invoke-direct {p0}, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;->n_swipeRight()V

    .line 85
    return-void
.end method

.method public swipeUp()V
    .locals 0

    .prologue
    .line 92
    invoke-direct {p0}, Lmono/android/inputmethodservice/KeyboardView_OnKeyboardActionListenerImplementor;->n_swipeUp()V

    .line 93
    return-void
.end method
