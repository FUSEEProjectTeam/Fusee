.class Lmono/android/app/Application$NotifyTimeZoneChanges;
.super Landroid/content/BroadcastReceiver;
.source "Application.java"


# annotations
.annotation system Ldalvik/annotation/EnclosingClass;
    value = Lmono/android/app/Application;
.end annotation

.annotation system Ldalvik/annotation/InnerClass;
    accessFlags = 0x8
    name = "NotifyTimeZoneChanges"
.end annotation


# direct methods
.method constructor <init>()V
    .locals 0

    .prologue
    .line 20
    invoke-direct {p0}, Landroid/content/BroadcastReceiver;-><init>()V

    return-void
.end method


# virtual methods
.method public onReceive(Landroid/content/Context;Landroid/content/Intent;)V
    .locals 0

    .prologue
    .line 23
    invoke-static {}, Lmono/android/Runtime;->notifyTimeZoneChanged()V

    .line 24
    return-void
.end method
