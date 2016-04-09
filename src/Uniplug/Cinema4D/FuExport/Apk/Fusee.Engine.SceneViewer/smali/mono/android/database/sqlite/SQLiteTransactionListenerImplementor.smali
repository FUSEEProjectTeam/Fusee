.class public Lmono/android/database/sqlite/SQLiteTransactionListenerImplementor;
.super Ljava/lang/Object;
.source "SQLiteTransactionListenerImplementor.java"

# interfaces
.implements Lmono/android/IGCUserPeer;
.implements Landroid/database/sqlite/SQLiteTransactionListener;


# static fields
.field static final __md_methods:Ljava/lang/String;


# instance fields
.field refList:Ljava/util/ArrayList;


# direct methods
.method static constructor <clinit>()V
    .locals 3

    .prologue
    .line 12
    const-string v0, "n_onBegin:()V:GetOnBeginHandler:Android.Database.Sqlite.ISQLiteTransactionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onCommit:()V:GetOnCommitHandler:Android.Database.Sqlite.ISQLiteTransactionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onRollback:()V:GetOnRollbackHandler:Android.Database.Sqlite.ISQLiteTransactionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n"

    sput-object v0, Lmono/android/database/sqlite/SQLiteTransactionListenerImplementor;->__md_methods:Ljava/lang/String;

    .line 17
    const-string v0, "Android.Database.Sqlite.ISQLiteTransactionListenerImplementor, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-class v1, Lmono/android/database/sqlite/SQLiteTransactionListenerImplementor;

    sget-object v2, Lmono/android/database/sqlite/SQLiteTransactionListenerImplementor;->__md_methods:Ljava/lang/String;

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
    invoke-direct {p0}, Ljava/lang/Object;-><init>()V

    .line 24
    invoke-virtual {p0}, Ljava/lang/Object;->getClass()Ljava/lang/Class;

    move-result-object v0

    const-class v1, Lmono/android/database/sqlite/SQLiteTransactionListenerImplementor;

    if-ne v0, v1, :cond_0

    .line 25
    const-string v0, "Android.Database.Sqlite.ISQLiteTransactionListenerImplementor, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-string v1, ""

    const/4 v2, 0x0

    new-array v2, v2, [Ljava/lang/Object;

    invoke-static {v0, v1, p0, v2}, Lmono/android/TypeManager;->Activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V

    .line 26
    :cond_0
    return-void
.end method

.method private native n_onBegin()V
.end method

.method private native n_onCommit()V
.end method

.method private native n_onRollback()V
.end method


# virtual methods
.method public monodroidAddReference(Ljava/lang/Object;)V
    .locals 1

    .prologue
    .line 55
    iget-object v0, p0, Lmono/android/database/sqlite/SQLiteTransactionListenerImplementor;->refList:Ljava/util/ArrayList;

    if-nez v0, :cond_0

    .line 56
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    iput-object v0, p0, Lmono/android/database/sqlite/SQLiteTransactionListenerImplementor;->refList:Ljava/util/ArrayList;

    .line 57
    :cond_0
    iget-object v0, p0, Lmono/android/database/sqlite/SQLiteTransactionListenerImplementor;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0, p1}, Ljava/util/ArrayList;->add(Ljava/lang/Object;)Z

    .line 58
    return-void
.end method

.method public monodroidClearReferences()V
    .locals 1

    .prologue
    .line 62
    iget-object v0, p0, Lmono/android/database/sqlite/SQLiteTransactionListenerImplementor;->refList:Ljava/util/ArrayList;

    if-eqz v0, :cond_0

    .line 63
    iget-object v0, p0, Lmono/android/database/sqlite/SQLiteTransactionListenerImplementor;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0}, Ljava/util/ArrayList;->clear()V

    .line 64
    :cond_0
    return-void
.end method

.method public onBegin()V
    .locals 0

    .prologue
    .line 31
    invoke-direct {p0}, Lmono/android/database/sqlite/SQLiteTransactionListenerImplementor;->n_onBegin()V

    .line 32
    return-void
.end method

.method public onCommit()V
    .locals 0

    .prologue
    .line 39
    invoke-direct {p0}, Lmono/android/database/sqlite/SQLiteTransactionListenerImplementor;->n_onCommit()V

    .line 40
    return-void
.end method

.method public onRollback()V
    .locals 0

    .prologue
    .line 47
    invoke-direct {p0}, Lmono/android/database/sqlite/SQLiteTransactionListenerImplementor;->n_onRollback()V

    .line 48
    return-void
.end method
