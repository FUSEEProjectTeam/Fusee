.class Lmono/MonoPackageManager_Resources;
.super Ljava/lang/Object;
.source "MonoPackageManager.java"


# static fields
.field public static final ApiPackageName:Ljava/lang/String;

.field public static final Assemblies:[Ljava/lang/String;

.field public static final Dependencies:[Ljava/lang/String;


# direct methods
.method static constructor <clinit>()V
    .locals 4

    .prologue
    const/4 v3, 0x0

    .line 81
    const/16 v0, 0x17

    new-array v0, v0, [Ljava/lang/String;

    const-string v1, "Fusee.Engine.SceneViewer.Android.dll"

    aput-object v1, v0, v3

    const/4 v1, 0x1

    const-string v2, "Fusee.Base.Common.dll"

    aput-object v2, v0, v1

    const/4 v1, 0x2

    const-string v2, "Fusee.Base.Core.dll"

    aput-object v2, v0, v1

    const/4 v1, 0x3

    const-string v2, "Fusee.Base.Imp.Android.dll"

    aput-object v2, v0, v1

    const/4 v1, 0x4

    const-string v2, "Fusee.Engine.Common.dll"

    aput-object v2, v0, v1

    const/4 v1, 0x5

    const-string v2, "Fusee.Engine.Core.dll"

    aput-object v2, v0, v1

    const/4 v1, 0x6

    const-string v2, "Fusee.Engine.Imp.Graphics.Android.dll"

    aput-object v2, v0, v1

    const/4 v1, 0x7

    const-string v2, "Fusee.Engine.SceneViewer.Core.dll"

    aput-object v2, v0, v1

    const/16 v1, 0x8

    const-string v2, "Fusee.Math.Core.dll"

    aput-object v2, v0, v1

    const/16 v1, 0x9

    const-string v2, "Fusee.Serialization.dll"

    aput-object v2, v0, v1

    const/16 v1, 0xa

    const-string v2, "Fusee.SerializationSerializer.dll"

    aput-object v2, v0, v1

    const/16 v1, 0xb

    const-string v2, "protobuf-net.dll"

    aput-object v2, v0, v1

    const/16 v1, 0xc

    const-string v2, "System.Diagnostics.Tracing.dll"

    aput-object v2, v0, v1

    const/16 v1, 0xd

    const-string v2, "System.Reflection.Emit.dll"

    aput-object v2, v0, v1

    const/16 v1, 0xe

    const-string v2, "System.Reflection.Emit.ILGeneration.dll"

    aput-object v2, v0, v1

    const/16 v1, 0xf

    const-string v2, "System.Reflection.Emit.Lightweight.dll"

    aput-object v2, v0, v1

    const/16 v1, 0x10

    const-string v2, "System.ServiceModel.Security.dll"

    aput-object v2, v0, v1

    const/16 v1, 0x11

    const-string v2, "System.Threading.Timer.dll"

    aput-object v2, v0, v1

    const/16 v1, 0x12

    const-string v2, "JSIL.Meta.dll"

    aput-object v2, v0, v1

    const/16 v1, 0x13

    const-string v2, "SharpFont.dll"

    aput-object v2, v0, v1

    const/16 v1, 0x14

    const-string v2, "Fusee.Xene.dll"

    aput-object v2, v0, v1

    const/16 v1, 0x15

    const-string v2, "Fusee.Xirkit.dll"

    aput-object v2, v0, v1

    const/16 v1, 0x16

    const-string v2, "System.ServiceModel.Internals.dll"

    aput-object v2, v0, v1

    sput-object v0, Lmono/MonoPackageManager_Resources;->Assemblies:[Ljava/lang/String;

    .line 106
    new-array v0, v3, [Ljava/lang/String;

    sput-object v0, Lmono/MonoPackageManager_Resources;->Dependencies:[Ljava/lang/String;

    .line 108
    const/4 v0, 0x0

    sput-object v0, Lmono/MonoPackageManager_Resources;->ApiPackageName:Ljava/lang/String;

    return-void
.end method

.method constructor <init>()V
    .locals 0

    .prologue
    .line 80
    invoke-direct {p0}, Ljava/lang/Object;-><init>()V

    return-void
.end method
