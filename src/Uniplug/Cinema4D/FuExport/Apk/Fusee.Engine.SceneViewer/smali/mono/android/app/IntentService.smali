.class public abstract Lmono/android/app/IntentService;
.super Landroid/app/IntentService;
.source "IntentService.java"


# instance fields
.field refList:Ljava/util/ArrayList;


# direct methods
.method public constructor <init>()V
    .locals 1

    .prologue
    .line 6
    const/4 v0, 0x0

    invoke-direct {p0, v0}, Lmono/android/app/IntentService;-><init>(Ljava/lang/String;)V

    .line 7
    return-void
.end method

.method public constructor <init>(Ljava/lang/String;)V
    .locals 0

    .prologue
    .line 11
    invoke-direct {p0, p1}, Landroid/app/IntentService;-><init>(Ljava/lang/String;)V

    .line 12
    return-void
.end method


# virtual methods
.method public monodroidAddReference(Ljava/lang/Object;)V
    .locals 1

    .prologue
    .line 17
    iget-object v0, p0, Lmono/android/app/IntentService;->refList:Ljava/util/ArrayList;

    if-nez v0, :cond_0

    .line 18
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    iput-object v0, p0, Lmono/android/app/IntentService;->refList:Ljava/util/ArrayList;

    .line 19
    :cond_0
    iget-object v0, p0, Lmono/android/app/IntentService;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0, p1}, Ljava/util/ArrayList;->add(Ljava/lang/Object;)Z

    .line 20
    return-void
.end method

.method public monodroidClearReferences()V
    .locals 1

    .prologue
    .line 24
    iget-object v0, p0, Lmono/android/app/IntentService;->refList:Ljava/util/ArrayList;

    if-eqz v0, :cond_0

    .line 25
    iget-object v0, p0, Lmono/android/app/IntentService;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0}, Ljava/util/ArrayList;->clear()V

    .line 26
    :cond_0
    return-void
.end method
