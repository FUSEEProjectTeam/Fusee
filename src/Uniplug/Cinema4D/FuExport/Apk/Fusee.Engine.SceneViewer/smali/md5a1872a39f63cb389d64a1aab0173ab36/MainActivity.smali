.class public Lmd5a1872a39f63cb389d64a1aab0173ab36/MainActivity;
.super Landroid/app/Activity;
.source "MainActivity.java"

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
    const-string v0, "n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n"

    sput-object v0, Lmd5a1872a39f63cb389d64a1aab0173ab36/MainActivity;->__md_methods:Ljava/lang/String;

    .line 14
    const-string v0, "Fusee.Engine.SceneViewer.Android.MainActivity, Fusee.Engine.SceneViewer.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"

    const-class v1, Lmd5a1872a39f63cb389d64a1aab0173ab36/MainActivity;

    sget-object v2, Lmd5a1872a39f63cb389d64a1aab0173ab36/MainActivity;->__md_methods:Ljava/lang/String;

    invoke-static {v0, v1, v2}, Lmono/android/Runtime;->register(Ljava/lang/String;Ljava/lang/Class;Ljava/lang/String;)V

    .line 15
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
    .line 20
    invoke-direct {p0}, Landroid/app/Activity;-><init>()V

    .line 21
    invoke-virtual {p0}, Ljava/lang/Object;->getClass()Ljava/lang/Class;

    move-result-object v0

    const-class v1, Lmd5a1872a39f63cb389d64a1aab0173ab36/MainActivity;

    if-ne v0, v1, :cond_0

    .line 22
    const-string v0, "Fusee.Engine.SceneViewer.Android.MainActivity, Fusee.Engine.SceneViewer.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"

    const-string v1, ""

    const/4 v2, 0x0

    new-array v2, v2, [Ljava/lang/Object;

    invoke-static {v0, v1, p0, v2}, Lmono/android/TypeManager;->Activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V

    .line 23
    :cond_0
    return-void
.end method

.method private native n_onCreate(Landroid/os/Bundle;)V
.end method


# virtual methods
.method public monodroidAddReference(Ljava/lang/Object;)V
    .locals 1

    .prologue
    .line 36
    iget-object v0, p0, Lmd5a1872a39f63cb389d64a1aab0173ab36/MainActivity;->refList:Ljava/util/ArrayList;

    if-nez v0, :cond_0

    .line 37
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    iput-object v0, p0, Lmd5a1872a39f63cb389d64a1aab0173ab36/MainActivity;->refList:Ljava/util/ArrayList;

    .line 38
    :cond_0
    iget-object v0, p0, Lmd5a1872a39f63cb389d64a1aab0173ab36/MainActivity;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0, p1}, Ljava/util/ArrayList;->add(Ljava/lang/Object;)Z

    .line 39
    return-void
.end method

.method public monodroidClearReferences()V
    .locals 1

    .prologue
    .line 43
    iget-object v0, p0, Lmd5a1872a39f63cb389d64a1aab0173ab36/MainActivity;->refList:Ljava/util/ArrayList;

    if-eqz v0, :cond_0

    .line 44
    iget-object v0, p0, Lmd5a1872a39f63cb389d64a1aab0173ab36/MainActivity;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0}, Ljava/util/ArrayList;->clear()V

    .line 45
    :cond_0
    return-void
.end method

.method public onCreate(Landroid/os/Bundle;)V
    .locals 0

    .prologue
    .line 28
    invoke-direct {p0, p1}, Lmd5a1872a39f63cb389d64a1aab0173ab36/MainActivity;->n_onCreate(Landroid/os/Bundle;)V

    .line 29
    return-void
.end method
