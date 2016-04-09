.class public Lmono/android/TypeManager;
.super Ljava/lang/Object;
.source "TypeManager.java"


# direct methods
.method static constructor <clinit>()V
    .locals 3

    .prologue
    .line 13
    const-string v0, "n_activate:(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V:GetActivateHandler\n"

    .line 16
    const-string v1, "Java.Interop.TypeManager, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"

    const-class v2, Lmono/android/TypeManager;

    invoke-static {v1, v2, v0}, Lmono/android/Runtime;->register(Ljava/lang/String;Ljava/lang/Class;Ljava/lang/String;)V

    .line 17
    return-void
.end method

.method public constructor <init>()V
    .locals 0

    .prologue
    .line 3
    invoke-direct {p0}, Ljava/lang/Object;-><init>()V

    return-void
.end method

.method public static Activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V
    .locals 0

    .prologue
    .line 7
    invoke-static {p0, p1, p2, p3}, Lmono/android/TypeManager;->n_activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V

    .line 8
    return-void
.end method

.method private static native n_activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V
.end method
