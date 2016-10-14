.class public Lmono/android/media/midi/MidiManager_OnDeviceOpenedListenerImplementor;
.super Ljava/lang/Object;
.source "MidiManager_OnDeviceOpenedListenerImplementor.java"

# interfaces
.implements Lmono/android/IGCUserPeer;
.implements Landroid/media/midi/MidiManager$OnDeviceOpenedListener;


# static fields
.field static final __md_methods:Ljava/lang/String;


# instance fields
.field refList:Ljava/util/ArrayList;


# direct methods
.method static constructor <clinit>()V
    .locals 3

    .prologue
    .line 12
    const-string v0, "n_onDeviceOpened:(Landroid/media/midi/MidiDevice;)V:GetOnDeviceOpened_Landroid_media_midi_MidiDevice_Handler:Android.Media.Midi.MidiManager/IOnDeviceOpenedListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n"

    sput-object v0, Lmono/android/media/midi/MidiManager_OnDeviceOpenedListenerImplementor;->__md_methods:Ljava/lang/String;

    .line 15
    const-string v0, "Android.Media.Midi.MidiManager+IOnDeviceOpenedListenerImplementor, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-class v1, Lmono/android/media/midi/MidiManager_OnDeviceOpenedListenerImplementor;

    sget-object v2, Lmono/android/media/midi/MidiManager_OnDeviceOpenedListenerImplementor;->__md_methods:Ljava/lang/String;

    invoke-static {v0, v1, v2}, Lmono/android/Runtime;->register(Ljava/lang/String;Ljava/lang/Class;Ljava/lang/String;)V

    .line 16
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
    .line 21
    invoke-direct {p0}, Ljava/lang/Object;-><init>()V

    .line 22
    invoke-virtual {p0}, Ljava/lang/Object;->getClass()Ljava/lang/Class;

    move-result-object v0

    const-class v1, Lmono/android/media/midi/MidiManager_OnDeviceOpenedListenerImplementor;

    if-ne v0, v1, :cond_0

    .line 23
    const-string v0, "Android.Media.Midi.MidiManager+IOnDeviceOpenedListenerImplementor, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-string v1, ""

    const/4 v2, 0x0

    new-array v2, v2, [Ljava/lang/Object;

    invoke-static {v0, v1, p0, v2}, Lmono/android/TypeManager;->Activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V

    .line 24
    :cond_0
    return-void
.end method

.method private native n_onDeviceOpened(Landroid/media/midi/MidiDevice;)V
.end method


# virtual methods
.method public monodroidAddReference(Ljava/lang/Object;)V
    .locals 1

    .prologue
    .line 37
    iget-object v0, p0, Lmono/android/media/midi/MidiManager_OnDeviceOpenedListenerImplementor;->refList:Ljava/util/ArrayList;

    if-nez v0, :cond_0

    .line 38
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    iput-object v0, p0, Lmono/android/media/midi/MidiManager_OnDeviceOpenedListenerImplementor;->refList:Ljava/util/ArrayList;

    .line 39
    :cond_0
    iget-object v0, p0, Lmono/android/media/midi/MidiManager_OnDeviceOpenedListenerImplementor;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0, p1}, Ljava/util/ArrayList;->add(Ljava/lang/Object;)Z

    .line 40
    return-void
.end method

.method public monodroidClearReferences()V
    .locals 1

    .prologue
    .line 44
    iget-object v0, p0, Lmono/android/media/midi/MidiManager_OnDeviceOpenedListenerImplementor;->refList:Ljava/util/ArrayList;

    if-eqz v0, :cond_0

    .line 45
    iget-object v0, p0, Lmono/android/media/midi/MidiManager_OnDeviceOpenedListenerImplementor;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0}, Ljava/util/ArrayList;->clear()V

    .line 46
    :cond_0
    return-void
.end method

.method public onDeviceOpened(Landroid/media/midi/MidiDevice;)V
    .locals 0

    .prologue
    .line 29
    invoke-direct {p0, p1}, Lmono/android/media/midi/MidiManager_OnDeviceOpenedListenerImplementor;->n_onDeviceOpened(Landroid/media/midi/MidiDevice;)V

    .line 30
    return-void
.end method
