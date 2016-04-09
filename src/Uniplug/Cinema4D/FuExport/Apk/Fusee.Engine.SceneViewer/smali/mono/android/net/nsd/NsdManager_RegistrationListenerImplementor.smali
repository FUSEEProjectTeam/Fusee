.class public Lmono/android/net/nsd/NsdManager_RegistrationListenerImplementor;
.super Ljava/lang/Object;
.source "NsdManager_RegistrationListenerImplementor.java"

# interfaces
.implements Lmono/android/IGCUserPeer;
.implements Landroid/net/nsd/NsdManager$RegistrationListener;


# static fields
.field static final __md_methods:Ljava/lang/String;


# instance fields
.field refList:Ljava/util/ArrayList;


# direct methods
.method static constructor <clinit>()V
    .locals 3

    .prologue
    .line 12
    const-string v0, "n_onRegistrationFailed:(Landroid/net/nsd/NsdServiceInfo;I)V:GetOnRegistrationFailed_Landroid_net_nsd_NsdServiceInfo_IHandler:Android.Net.Nsd.NsdManager/IRegistrationListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onServiceRegistered:(Landroid/net/nsd/NsdServiceInfo;)V:GetOnServiceRegistered_Landroid_net_nsd_NsdServiceInfo_Handler:Android.Net.Nsd.NsdManager/IRegistrationListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onServiceUnregistered:(Landroid/net/nsd/NsdServiceInfo;)V:GetOnServiceUnregistered_Landroid_net_nsd_NsdServiceInfo_Handler:Android.Net.Nsd.NsdManager/IRegistrationListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\nn_onUnregistrationFailed:(Landroid/net/nsd/NsdServiceInfo;I)V:GetOnUnregistrationFailed_Landroid_net_nsd_NsdServiceInfo_IHandler:Android.Net.Nsd.NsdManager/IRegistrationListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n"

    sput-object v0, Lmono/android/net/nsd/NsdManager_RegistrationListenerImplementor;->__md_methods:Ljava/lang/String;

    .line 18
    const-string v0, "Android.Net.Nsd.NsdManager+IRegistrationListenerImplementor, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-class v1, Lmono/android/net/nsd/NsdManager_RegistrationListenerImplementor;

    sget-object v2, Lmono/android/net/nsd/NsdManager_RegistrationListenerImplementor;->__md_methods:Ljava/lang/String;

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
    invoke-direct {p0}, Ljava/lang/Object;-><init>()V

    .line 25
    invoke-virtual {p0}, Ljava/lang/Object;->getClass()Ljava/lang/Class;

    move-result-object v0

    const-class v1, Lmono/android/net/nsd/NsdManager_RegistrationListenerImplementor;

    if-ne v0, v1, :cond_0

    .line 26
    const-string v0, "Android.Net.Nsd.NsdManager+IRegistrationListenerImplementor, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"

    const-string v1, ""

    const/4 v2, 0x0

    new-array v2, v2, [Ljava/lang/Object;

    invoke-static {v0, v1, p0, v2}, Lmono/android/TypeManager;->Activate(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Object;[Ljava/lang/Object;)V

    .line 27
    :cond_0
    return-void
.end method

.method private native n_onRegistrationFailed(Landroid/net/nsd/NsdServiceInfo;I)V
.end method

.method private native n_onServiceRegistered(Landroid/net/nsd/NsdServiceInfo;)V
.end method

.method private native n_onServiceUnregistered(Landroid/net/nsd/NsdServiceInfo;)V
.end method

.method private native n_onUnregistrationFailed(Landroid/net/nsd/NsdServiceInfo;I)V
.end method


# virtual methods
.method public monodroidAddReference(Ljava/lang/Object;)V
    .locals 1

    .prologue
    .line 64
    iget-object v0, p0, Lmono/android/net/nsd/NsdManager_RegistrationListenerImplementor;->refList:Ljava/util/ArrayList;

    if-nez v0, :cond_0

    .line 65
    new-instance v0, Ljava/util/ArrayList;

    invoke-direct {v0}, Ljava/util/ArrayList;-><init>()V

    iput-object v0, p0, Lmono/android/net/nsd/NsdManager_RegistrationListenerImplementor;->refList:Ljava/util/ArrayList;

    .line 66
    :cond_0
    iget-object v0, p0, Lmono/android/net/nsd/NsdManager_RegistrationListenerImplementor;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0, p1}, Ljava/util/ArrayList;->add(Ljava/lang/Object;)Z

    .line 67
    return-void
.end method

.method public monodroidClearReferences()V
    .locals 1

    .prologue
    .line 71
    iget-object v0, p0, Lmono/android/net/nsd/NsdManager_RegistrationListenerImplementor;->refList:Ljava/util/ArrayList;

    if-eqz v0, :cond_0

    .line 72
    iget-object v0, p0, Lmono/android/net/nsd/NsdManager_RegistrationListenerImplementor;->refList:Ljava/util/ArrayList;

    invoke-virtual {v0}, Ljava/util/ArrayList;->clear()V

    .line 73
    :cond_0
    return-void
.end method

.method public onRegistrationFailed(Landroid/net/nsd/NsdServiceInfo;I)V
    .locals 0

    .prologue
    .line 32
    invoke-direct {p0, p1, p2}, Lmono/android/net/nsd/NsdManager_RegistrationListenerImplementor;->n_onRegistrationFailed(Landroid/net/nsd/NsdServiceInfo;I)V

    .line 33
    return-void
.end method

.method public onServiceRegistered(Landroid/net/nsd/NsdServiceInfo;)V
    .locals 0

    .prologue
    .line 40
    invoke-direct {p0, p1}, Lmono/android/net/nsd/NsdManager_RegistrationListenerImplementor;->n_onServiceRegistered(Landroid/net/nsd/NsdServiceInfo;)V

    .line 41
    return-void
.end method

.method public onServiceUnregistered(Landroid/net/nsd/NsdServiceInfo;)V
    .locals 0

    .prologue
    .line 48
    invoke-direct {p0, p1}, Lmono/android/net/nsd/NsdManager_RegistrationListenerImplementor;->n_onServiceUnregistered(Landroid/net/nsd/NsdServiceInfo;)V

    .line 49
    return-void
.end method

.method public onUnregistrationFailed(Landroid/net/nsd/NsdServiceInfo;I)V
    .locals 0

    .prologue
    .line 56
    invoke-direct {p0, p1, p2}, Lmono/android/net/nsd/NsdManager_RegistrationListenerImplementor;->n_onUnregistrationFailed(Landroid/net/nsd/NsdServiceInfo;I)V

    .line 57
    return-void
.end method
