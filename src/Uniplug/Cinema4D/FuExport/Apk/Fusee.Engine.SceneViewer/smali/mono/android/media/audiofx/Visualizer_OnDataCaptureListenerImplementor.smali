.class public Lmono/android/media/audiofx/Visualizer_OnDataCaptureListenerImplementor;
.super Ljava/lang/Object;
.source "Visualizer_OnDataCaptureListenerImplementor.java"

# interfaces
.implements Lmono/android/IGCUserPeer;
.implements Landroid/media/audiofx/Visualizer$OnDataCaptureListener;


# static fields
.field static final __md_methods:Ljava/lang/String;


# instance fields
.field refList:Ljava/util/ArrayList;


# direct methods
.method static constructor <clinit>()V
    .locals 3

    .prologue
    .line 12
    const-string v0, "n_onFftDataCapture:(Landroid/media/audiofx/Visualizer;[BI)V:GetOnFftDataCapture_Landroid_media_audiofx_Visualizer_arrayBIHandler:Android.Media.Audiofx.Visualizer/IOnDataCaptureListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onWaveFormDataCapture:(Landroid/media/audiofx/Visualizer;[BI)V:GetOnWaveFormDataCapture_Landroid_media_audiofx_Visualizer_arrayBIHandler:Android.Media.Audiofx.Visualizer/IOnDataCaptureListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n"

    sput-object v0, Lmono/android/media/audiofx/Visualizer_OnDataCaptureListenerImplementor;->__md_methods:Ljava/lang/String;

    .line 16
    const-string v0, "Android.Media.Audiofx.Visualizer+IOnDataCaptureListenerImplementor, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-class v1, Lmono/android/media/audiofx/Visualizer_OnDataCaptureListenerImplementor;

    sget-object v2, Lmono/android/media/audiofx/Visualizer_OnDataCaptureListenerImplementor;->__md_methods:Ljava/lang/String;

    invoke-static {v0, v1, v2}, Lmono/android/Runtime;->register(Ljava/lang/String;Ljava/lang/Class;Ljava/lang/String;)V

    .line 17
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
    .line 22
    invoke-direct {p0}, Ljava/lang/Object;-><init>()V

    .line 23
    invoke-virtual {p0}, Ljava/lang/Object;->getClass()Ljava/lang/Class;

    move-result-object v0

    const-class v1, Lmono/android/media/audiofx/Visualizer_OnDataCaptureListenerImplementor;

    if-ne v0, v1, :cond_0

    .line 24
    const-string v0, "Android.Media.Audiofx.Visualizer+IOnDataCaptureListenerImplementor, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-string v1, ""

    const/4 v2, 0x0

    new-array v2, v2, [Ljava/lang/Object;

    invoke-static {v0, v1, p0, v2}, Lmono/android/TypeManager;->Activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V

    .line 25
    :cond_0
    return-void
.end method

.method private native n_onFftDataCapture(Landroid/media/audiofx/Visualizer;[BI)V
.end method

.method private native n_onWaveFormDataCapture(Landroid/media/audiofx/Visualizer;[BI)V
.end method


# virtual methods
.method public monodroidAddReference(Ljava/lang/Object;)V
    .locals 1

    .prologue
    .line 46
    iget-object v0, p0, Lmono/android/media/audiofx/Visualizer_OnDataCaptureListenerImplementor;->refList:Ljava/util/ArrayList;

    if-nez v0, :cond_0

    .line 47
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    iput-object v0, p0, Lmono/android/media/audiofx/Visualizer_OnDataCaptureListenerImplementor;->refList:Ljava/util/ArrayList;

    .line 48
    :cond_0
    iget-object v0, p0, Lmono/android/media/audiofx/Visualizer_OnDataCaptureListenerImplementor;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0, p1}, Ljava/util/ArrayList;->add(Ljava/lang/Object;)Z

    .line 49
    return-void
.end method

.method public monodroidClearReferences()V
    .locals 1

    .prologue
    .line 53
    iget-object v0, p0, Lmono/android/media/audiofx/Visualizer_OnDataCaptureListenerImplementor;->refList:Ljava/util/ArrayList;

    if-eqz v0, :cond_0

    .line 54
    iget-object v0, p0, Lmono/android/media/audiofx/Visualizer_OnDataCaptureListenerImplementor;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0}, Ljava/util/ArrayList;->clear()V

    .line 55
    :cond_0
    return-void
.end method

.method public onFftDataCapture(Landroid/media/audiofx/Visualizer;[BI)V
    .locals 0

    .prologue
    .line 30
    invoke-direct {p0, p1, p2, p3}, Lmono/android/media/audiofx/Visualizer_OnDataCaptureListenerImplementor;->n_onFftDataCapture(Landroid/media/audiofx/Visualizer;[BI)V

    .line 31
    return-void
.end method

.method public onWaveFormDataCapture(Landroid/media/audiofx/Visualizer;[BI)V
    .locals 0

    .prologue
    .line 38
    invoke-direct {p0, p1, p2, p3}, Lmono/android/media/audiofx/Visualizer_OnDataCaptureListenerImplementor;->n_onWaveFormDataCapture(Landroid/media/audiofx/Visualizer;[BI)V

    .line 39
    return-void
.end method
