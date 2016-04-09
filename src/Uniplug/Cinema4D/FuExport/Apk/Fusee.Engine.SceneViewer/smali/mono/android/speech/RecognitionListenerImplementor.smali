.class public Lmono/android/speech/RecognitionListenerImplementor;
.super Ljava/lang/Object;
.source "RecognitionListenerImplementor.java"

# interfaces
.implements Lmono/android/IGCUserPeer;
.implements Landroid/speech/RecognitionListener;


# static fields
.field static final __md_methods:Ljava/lang/String;


# instance fields
.field refList:Ljava/util/ArrayList;


# direct methods
.method static constructor <clinit>()V
    .locals 3

    .prologue
    .line 12
    const-string v0, "n_onBeginningOfSpeech:()V:GetOnBeginningOfSpeechHandler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onBufferReceived:([B)V:GetOnBufferReceived_arrayBHandler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onEndOfSpeech:()V:GetOnEndOfSpeechHandler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onError:(I)V:GetOnError_IHandler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onEvent:(ILandroid/os/Bundle;)V:GetOnEvent_ILandroid_os_Bundle_Handler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onPartialResults:(Landroid/os/Bundle;)V:GetOnPartialResults_Landroid_os_Bundle_Handler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onReadyForSpeech:(Landroid/os/Bundle;)V:GetOnReadyForSpeech_Landroid_os_Bundle_Handler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onResults:(Landroid/os/Bundle;)V:GetOnResults_Landroid_os_Bundle_Handler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onRmsChanged:(F)V:GetOnRmsChanged_FHandler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n"

    sput-object v0, Lmono/android/speech/RecognitionListenerImplementor;->__md_methods:Ljava/lang/String;

    .line 23
    const-string v0, "Android.Speech.IRecognitionListenerImplementor, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-class v1, Lmono/android/speech/RecognitionListenerImplementor;

    sget-object v2, Lmono/android/speech/RecognitionListenerImplementor;->__md_methods:Ljava/lang/String;

    invoke-static {v0, v1, v2}, Lmono/android/Runtime;->register(Ljava/lang/String;Ljava/lang/Class;Ljava/lang/String;)V

    .line 24
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
    .line 29
    invoke-direct {p0}, Ljava/lang/Object;-><init>()V

    .line 30
    invoke-virtual {p0}, Ljava/lang/Object;->getClass()Ljava/lang/Class;

    move-result-object v0

    const-class v1, Lmono/android/speech/RecognitionListenerImplementor;

    if-ne v0, v1, :cond_0

    .line 31
    const-string v0, "Android.Speech.IRecognitionListenerImplementor, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-string v1, ""

    const/4 v2, 0x0

    new-array v2, v2, [Ljava/lang/Object;

    invoke-static {v0, v1, p0, v2}, Lmono/android/TypeManager;->Activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V

    .line 32
    :cond_0
    return-void
.end method

.method private native n_onBeginningOfSpeech()V
.end method

.method private native n_onBufferReceived([B)V
.end method

.method private native n_onEndOfSpeech()V
.end method

.method private native n_onError(I)V
.end method

.method private native n_onEvent(ILandroid/os/Bundle;)V
.end method

.method private native n_onPartialResults(Landroid/os/Bundle;)V
.end method

.method private native n_onReadyForSpeech(Landroid/os/Bundle;)V
.end method

.method private native n_onResults(Landroid/os/Bundle;)V
.end method

.method private native n_onRmsChanged(F)V
.end method


# virtual methods
.method public monodroidAddReference(Ljava/lang/Object;)V
    .locals 1

    .prologue
    .line 109
    iget-object v0, p0, Lmono/android/speech/RecognitionListenerImplementor;->refList:Ljava/util/ArrayList;

    if-nez v0, :cond_0

    .line 110
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    iput-object v0, p0, Lmono/android/speech/RecognitionListenerImplementor;->refList:Ljava/util/ArrayList;

    .line 111
    :cond_0
    iget-object v0, p0, Lmono/android/speech/RecognitionListenerImplementor;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0, p1}, Ljava/util/ArrayList;->add(Ljava/lang/Object;)Z

    .line 112
    return-void
.end method

.method public monodroidClearReferences()V
    .locals 1

    .prologue
    .line 116
    iget-object v0, p0, Lmono/android/speech/RecognitionListenerImplementor;->refList:Ljava/util/ArrayList;

    if-eqz v0, :cond_0

    .line 117
    iget-object v0, p0, Lmono/android/speech/RecognitionListenerImplementor;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0}, Ljava/util/ArrayList;->clear()V

    .line 118
    :cond_0
    return-void
.end method

.method public onBeginningOfSpeech()V
    .locals 0

    .prologue
    .line 37
    invoke-direct {p0}, Lmono/android/speech/RecognitionListenerImplementor;->n_onBeginningOfSpeech()V

    .line 38
    return-void
.end method

.method public onBufferReceived([B)V
    .locals 0

    .prologue
    .line 45
    invoke-direct {p0, p1}, Lmono/android/speech/RecognitionListenerImplementor;->n_onBufferReceived([B)V

    .line 46
    return-void
.end method

.method public onEndOfSpeech()V
    .locals 0

    .prologue
    .line 53
    invoke-direct {p0}, Lmono/android/speech/RecognitionListenerImplementor;->n_onEndOfSpeech()V

    .line 54
    return-void
.end method

.method public onError(I)V
    .locals 0

    .prologue
    .line 61
    invoke-direct {p0, p1}, Lmono/android/speech/RecognitionListenerImplementor;->n_onError(I)V

    .line 62
    return-void
.end method

.method public onEvent(ILandroid/os/Bundle;)V
    .locals 0

    .prologue
    .line 69
    invoke-direct {p0, p1, p2}, Lmono/android/speech/RecognitionListenerImplementor;->n_onEvent(ILandroid/os/Bundle;)V

    .line 70
    return-void
.end method

.method public onPartialResults(Landroid/os/Bundle;)V
    .locals 0

    .prologue
    .line 77
    invoke-direct {p0, p1}, Lmono/android/speech/RecognitionListenerImplementor;->n_onPartialResults(Landroid/os/Bundle;)V

    .line 78
    return-void
.end method

.method public onReadyForSpeech(Landroid/os/Bundle;)V
    .locals 0

    .prologue
    .line 85
    invoke-direct {p0, p1}, Lmono/android/speech/RecognitionListenerImplementor;->n_onReadyForSpeech(Landroid/os/Bundle;)V

    .line 86
    return-void
.end method

.method public onResults(Landroid/os/Bundle;)V
    .locals 0

    .prologue
    .line 93
    invoke-direct {p0, p1}, Lmono/android/speech/RecognitionListenerImplementor;->n_onResults(Landroid/os/Bundle;)V

    .line 94
    return-void
.end method

.method public onRmsChanged(F)V
    .locals 0

    .prologue
    .line 101
    invoke-direct {p0, p1}, Lmono/android/speech/RecognitionListenerImplementor;->n_onRmsChanged(F)V

    .line 102
    return-void
.end method
