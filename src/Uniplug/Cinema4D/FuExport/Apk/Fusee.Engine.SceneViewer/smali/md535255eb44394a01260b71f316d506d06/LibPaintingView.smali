.class public Lmd535255eb44394a01260b71f316d506d06/LibPaintingView;
.super Lopentk/platform/android/AndroidGameView;
.source "LibPaintingView.java"

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
    const-string v0, ""

    sput-object v0, Lmd535255eb44394a01260b71f316d506d06/LibPaintingView;->__md_methods:Ljava/lang/String;

    .line 13
    const-string v0, "Fusee.Engine.Imp.Graphics.Android.LibPaintingView, Fusee.Engine.Imp.Graphics.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"

    const-class v1, Lmd535255eb44394a01260b71f316d506d06/LibPaintingView;

    sget-object v2, Lmd535255eb44394a01260b71f316d506d06/LibPaintingView;->__md_methods:Ljava/lang/String;

    invoke-static {v0, v1, v2}, Lmono/android/Runtime;->register(Ljava/lang/String;Ljava/lang/Class;Ljava/lang/String;)V

    .line 14
    return-void
.end method

.method public constructor <init>(Landroid/content/Context;Landroid/util/AttributeSet;)V
    .locals 4
    .annotation system Ldalvik/annotation/Throws;
        value = {
            Ljava/lang/Throwable;
        }
    .end annotation

    .prologue
    .line 19
    invoke-direct {p0, p1, p2}, Lopentk/platform/android/AndroidGameView;-><init>(Landroid/content/Context;Landroid/util/AttributeSet;)V

    .line 20
    invoke-virtual {p0}, Ljava/lang/Object;->getClass()Ljava/lang/Class;

    move-result-object v0

    const-class v1, Lmd535255eb44394a01260b71f316d506d06/LibPaintingView;

    if-ne v0, v1, :cond_0

    .line 21
    const-string v0, "Fusee.Engine.Imp.Graphics.Android.LibPaintingView, Fusee.Engine.Imp.Graphics.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"

    const-string v1, "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const/4 v2, 0x2

    new-array v2, v2, [Ljava/lang/Object;

    const/4 v3, 0x0

    aput-object p1, v2, v3

    const/4 v3, 0x1

    aput-object p2, v2, v3

    invoke-static {v0, v1, p0, v2}, Lmono/android/TypeManager;->Activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V

    .line 22
    :cond_0
    return-void
.end method

.method public constructor <init>(Landroid/content/Context;Landroid/util/AttributeSet;I)V
    .locals 5
    .annotation system Ldalvik/annotation/Throws;
        value = {
            Ljava/lang/Throwable;
        }
    .end annotation

    .prologue
    .line 27
    invoke-direct {p0, p1, p2, p3}, Lopentk/platform/android/AndroidGameView;-><init>(Landroid/content/Context;Landroid/util/AttributeSet;I)V

    .line 28
    invoke-virtual {p0}, Ljava/lang/Object;->getClass()Ljava/lang/Class;

    move-result-object v0

    const-class v1, Lmd535255eb44394a01260b71f316d506d06/LibPaintingView;

    if-ne v0, v1, :cond_0

    .line 29
    const-string v0, "Fusee.Engine.Imp.Graphics.Android.LibPaintingView, Fusee.Engine.Imp.Graphics.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"

    const-string v1, "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"

    const/4 v2, 0x3

    new-array v2, v2, [Ljava/lang/Object;

    const/4 v3, 0x0

    aput-object p1, v2, v3

    const/4 v3, 0x1

    aput-object p2, v2, v3

    const/4 v3, 0x2

    invoke-static {p3}, Ljava/lang/Integer;->valueOf(I)Ljava/lang/Integer;

    move-result-object v4

    aput-object v4, v2, v3

    invoke-static {v0, v1, p0, v2}, Lmono/android/TypeManager;->Activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V

    .line 30
    :cond_0
    return-void
.end method


# virtual methods
.method public monodroidAddReference(Ljava/lang/Object;)V
    .locals 1

    .prologue
    .line 35
    iget-object v0, p0, Lmd535255eb44394a01260b71f316d506d06/LibPaintingView;->refList:Ljava/util/ArrayList;

    if-nez v0, :cond_0

    .line 36
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    iput-object v0, p0, Lmd535255eb44394a01260b71f316d506d06/LibPaintingView;->refList:Ljava/util/ArrayList;

    .line 37
    :cond_0
    iget-object v0, p0, Lmd535255eb44394a01260b71f316d506d06/LibPaintingView;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0, p1}, Ljava/util/ArrayList;->add(Ljava/lang/Object;)Z

    .line 38
    return-void
.end method

.method public monodroidClearReferences()V
    .locals 1

    .prologue
    .line 42
    iget-object v0, p0, Lmd535255eb44394a01260b71f316d506d06/LibPaintingView;->refList:Ljava/util/ArrayList;

    if-eqz v0, :cond_0

    .line 43
    iget-object v0, p0, Lmd535255eb44394a01260b71f316d506d06/LibPaintingView;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0}, Ljava/util/ArrayList;->clear()V

    .line 44
    :cond_0
    return-void
.end method
