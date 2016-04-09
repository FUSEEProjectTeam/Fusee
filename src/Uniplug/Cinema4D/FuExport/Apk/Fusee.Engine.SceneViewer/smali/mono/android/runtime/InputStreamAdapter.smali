.class public Lmono/android/runtime/InputStreamAdapter;
.super Ljava/io/InputStream;
.source "InputStreamAdapter.java"

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
    const-string v0, "n_close:()V:GetCloseHandler\nn_read:()I:GetReadHandler\nn_read:([B)I:GetRead_arrayBHandler\nn_read:([BII)I:GetRead_arrayBIIHandler\n"

    sput-object v0, Lmono/android/runtime/InputStreamAdapter;->__md_methods:Ljava/lang/String;

    .line 17
    const-string v0, "Android.Runtime.InputStreamAdapter, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-class v1, Lmono/android/runtime/InputStreamAdapter;

    sget-object v2, Lmono/android/runtime/InputStreamAdapter;->__md_methods:Ljava/lang/String;

    invoke-static {v0, v1, v2}, Lmono/android/Runtime;->register(Ljava/lang/String;Ljava/lang/Class;Ljava/lang/String;)V

    .line 18
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
    .line 23
    invoke-direct {p0}, Ljava/io/InputStream;-><init>()V

    .line 24
    invoke-virtual {p0}, Ljava/lang/Object;->getClass()Ljava/lang/Class;

    move-result-object v0

    const-class v1, Lmono/android/runtime/InputStreamAdapter;

    if-ne v0, v1, :cond_0

    .line 25
    const-string v0, "Android.Runtime.InputStreamAdapter, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-string v1, ""

    const/4 v2, 0x0

    new-array v2, v2, [Ljava/lang/Object;

    invoke-static {v0, v1, p0, v2}, Lmono/android/TypeManager;->Activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V

    .line 26
    :cond_0
    return-void
.end method

.method private native n_close()V
.end method

.method private native n_read()I
.end method

.method private native n_read([B)I
.end method

.method private native n_read([BII)I
.end method


# virtual methods
.method public close()V
    .locals 0

    .prologue
    .line 31
    invoke-direct {p0}, Lmono/android/runtime/InputStreamAdapter;->n_close()V

    .line 32
    return-void
.end method

.method public monodroidAddReference(Ljava/lang/Object;)V
    .locals 1

    .prologue
    .line 63
    iget-object v0, p0, Lmono/android/runtime/InputStreamAdapter;->refList:Ljava/util/ArrayList;

    if-nez v0, :cond_0

    .line 64
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    iput-object v0, p0, Lmono/android/runtime/InputStreamAdapter;->refList:Ljava/util/ArrayList;

    .line 65
    :cond_0
    iget-object v0, p0, Lmono/android/runtime/InputStreamAdapter;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0, p1}, Ljava/util/ArrayList;->add(Ljava/lang/Object;)Z

    .line 66
    return-void
.end method

.method public monodroidClearReferences()V
    .locals 1

    .prologue
    .line 70
    iget-object v0, p0, Lmono/android/runtime/InputStreamAdapter;->refList:Ljava/util/ArrayList;

    if-eqz v0, :cond_0

    .line 71
    iget-object v0, p0, Lmono/android/runtime/InputStreamAdapter;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0}, Ljava/util/ArrayList;->clear()V

    .line 72
    :cond_0
    return-void
.end method

.method public read()I
    .locals 1

    .prologue
    .line 39
    invoke-direct {p0}, Lmono/android/runtime/InputStreamAdapter;->n_read()I

    move-result v0

    return v0
.end method

.method public read([B)I
    .locals 1

    .prologue
    .line 47
    invoke-direct {p0, p1}, Lmono/android/runtime/InputStreamAdapter;->n_read([B)I

    move-result v0

    return v0
.end method

.method public read([BII)I
    .locals 1

    .prologue
    .line 55
    invoke-direct {p0, p1, p2, p3}, Lmono/android/runtime/InputStreamAdapter;->n_read([BII)I

    move-result v0

    return v0
.end method
