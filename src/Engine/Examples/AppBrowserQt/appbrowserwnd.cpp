#include "appbrowserwnd.h"
#include "ui_appbrowserwnd.h"
#include "qmessagebox.h"
// #include "QtHostBridge.h"
#include "QtHost.h"


using namespace FuseeQtHostBridge;

AppBrowserWnd::AppBrowserWnd(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::AppBrowserWnd)
{
    ui->setupUi(this);

}

AppBrowserWnd::~AppBrowserWnd()
{
    delete ui;
}


void AppBrowserWnd::on_pushButton_clicked()
{

    //int ret = QMessageBox::information( 0,  "TEst",
    //                                "Druckmichdruckt",
    //                                QMessageBox::Ok | QMessageBox::Default );
	// MyClass *pMc = new MyClass();
	// pMc->SimpleMethod(this->ui->renderParent->winId());

	QtHost *pQH = new QtHost(this->ui->renderParent);
}