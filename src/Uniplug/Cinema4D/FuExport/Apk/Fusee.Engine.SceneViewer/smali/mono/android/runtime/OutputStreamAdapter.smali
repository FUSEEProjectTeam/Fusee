.class public Lmono/android/runtime/OutputStreamAdapter;
.super Ljava/io/OutputStream;
.source "OutputStreamAdapter.java"

# interfaces
.implements Lmono/android/IGCUserPeer;


# static fields
.field static final __md_methods:Ljava/lang/String;


# instance fields
.field refList:Ljava/util/ArrayList;


# direct methods
.method static constructor <clinit>()V
    .locals 3

    .prologue
    .line 11
    const-string v0, "n_close:()V:GetCloseHandler\nn_flush:()V:GetFlushHandler\nn_write:([B)V:GetWrite_arrayBHandler\nn_write:([BII)V:GetWrite_arrayBIIHandler\nn_write:(I)V:GetWrite_IHandler\n"

    sput-object v0, Lmono/android/runtime/OutputStreamAdapter;->__md_methods:Ljava/lang/String;

    .line 18
    const-string v0, "Android.Runtime.OutputStreamAdapter, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-class v1, Lmono/android/runtime/OutputStreamAdapter;

    sget-object v2, Lmono/android/runtime/OutputStreamAdapter;->__md_methods:Ljava/lang/String;

    invoke-static {v0, v1, v2}, Lmono/android/Runtime;->register(Ljava/lang/String;Ljava/lang/Class;Ljava/lang/String;)V

    .line 19
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
    .line 24
    invoke-direct {p0}, Ljava/io/OutputStream;-><init>()V

    .line 25
    invoke-virtual {p0}, Ljava/lang/Object;->getClass()Ljava/lang/Class;

    move-result-object v0

    const-class v1, Lmono/android/runtime/OutputStreamAdapter;

    if-ne v0, v1, :cond_0

    .line 26
    const-string v0, "Android.Runtime.OutputStreamAdapter, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-string v1, ""

    const/4 v2, 0x0

    new-array v2, v2, [Ljava/lang/Object;

    invoke-static {v0, v1, p0, v2}, Lmono/android/TypeManager;->Activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V

    .line 27
    :cond_0
    return-void
.end method

.method private native n_close()V
.end method

.method private native n_flush()V
.end method

.method private native n_write(I)V
.end method

.method private native n_write([B)V
.end method

.method private native n_write([BII)V
.end method


# virtual methods
.method public close()V
    .locals 0

    .prologue
    .line 32
    invoke-direct {p0}, Lmono/android/runtime/OutputStreamAdapter;->n_close()V

    .line 33
    return-void
.end method

.method public flush()V
    .locals 0

    .prologue
    .line 40
    invoke-direct {p0}, Lmono/android/runtime/OutputStreamAdapter;->n_flush()V

    .line 41
    return-void
.end method

.method public monodroidAddReference(Ljava/lang/Object;)V
    .locals 1

    .prologue
    .line 72
    iget-object v0, p0, Lmono/android/runtime/OutputStreamAdapter;->refList:Ljava/util/ArrayList;

    if-nez v0, :cond_0

    .line 73
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    iput-object v0, p0, Lmono/android/runtime/OutputStreamAdapter;->refList:Ljava/util/ArrayList;

    .line 74
    :cond_0
    iget-object v0, p0, Lmono/android/runtime/OutputStreamAdapter;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0, p1}, Ljava/util/ArrayList;->add(Ljava/lang/Object;)Z

    .line 75
    return-void
.end method

.method public monodroidClearReferences()V
    .locals 1

    .prologue
    .line 79
    iget-object v0, p0, Lmono/android/runtime/OutputStreamAdapter;->refList:Ljava/util/ArrayList;

    if-eqz v0, :cond_0

    .line 80
    iget-object v0, p0, Lmono/android/runtime/OutputStreamAdapter;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0}, Ljava/util/ArrayList;->clear()V

    .line 81
    :cond_0
    return-void
.end method

.method public write(I)V
    .locals 0

    .prologue
    .line 64
    invoke-direct {p0, p1}, Lmono/android/runtime/OutputStreamAdapter;->n_write(I)V

    .line 65
    return-void
.end method

.method public write([B)V
    .locals 0

    .prologue
    .line 48
    invoke-direct {p0, p1}, Lmono/android/runtime/OutputStreamAdapter;->n_write([B)V

    .line 49
    return-void
.end method

.method public write([BII)V
    .locals 0

    .prologue
    .line 56
    invoke-direct {p0, p1, p2, p3}, Lmono/android/runtime/OutputStreamAdapter;->n_write([BII)V

    .line 57
    return-void
.end method
