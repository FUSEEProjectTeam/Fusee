#include "QtApplication.h"
#include "ui_QtApplication.h"
#include "qmessagebox.h"
// #include "QtHostBridge.h"
#include "FuseeWidget.h"

using namespace FuseeQtHostBridge;

QtApplication::QtApplication(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::QtApplication)
{
    ui->setupUi(this);

}

QtApplication::~QtApplication()
{
    delete ui;
}


void QtApplication::on_pushButton_clicked()
{

    //int ret = QMessageBox::information( 0,  "TEst",
    //                                "Druckmichdruckt",
    //                                QMessageBox::Ok | QMessageBox::Default );
	// MyClass *pMc = new MyClass();
	// pMc->SimpleMethod(this->ui->renderParent->winId());

	FuseeWidget *pQH = new FuseeWidget(this->ui->renderParent);

	/*
	FuseeAppBridge pFuseeApp = new FuseeAppBridge(pQH);
	pFuseeApp->Run();
	*/
}