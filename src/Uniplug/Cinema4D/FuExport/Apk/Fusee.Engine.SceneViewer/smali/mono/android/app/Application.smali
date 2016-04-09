.class public Lmono/android/app/Application;
.super Landroid/app/Application;
.source "Application.java"


# annotations
.annotation system Ldalvik/annotation/MemberClasses;
    value = {
        Lmono/android/app/Application$NotifyTimeZoneChanges;
    }
.end annotation


# static fields
.field static Context:Landroid/content/Context;


# direct methods
.method public constructor <init>()V
    .locals 0

    .prologue
    .line 6
    invoke-direct {p0}, Landroid/app/Application;-><init>()V

    .line 7
    sput-object p0, Lmono/android/app/Application;->Context:Landroid/content/Context;

    .line 8
    return-void
.end method


# virtual methods
.method public onCreate()V
    .locals 2

    .prologue
    .line 14
    new-instance v0, Landroid/content/IntentFilter;

    const-string v1, "android.intent.action.TIMEZONE_CHANGED"

    invoke-direct {v0, v1}, Landroid/content/IntentFilter;-><init>(Ljava/lang/String;)V

    .line 17
    new-instance v1, Lmono/android/app/Application$NotifyTimeZoneChanges;

    invoke-direct {v1}, Lmono/android/app/Application$NotifyTimeZoneChanges;-><init>()V

    invoke-virtual {p0, v1, v0}, Lmono/android/app/Application;->registerReceiver(Landroid/content/BroadcastReceiver;Landroid/content/IntentFilter;)Landroid/content/Intent;

    .line 18
    return-void
.end method
