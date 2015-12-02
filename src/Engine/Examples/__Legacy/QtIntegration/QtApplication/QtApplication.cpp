#include "QtApplication.h"
#include "ui_QtApplication.h"
#include "qmessagebox.h"
// #include "QtHostBridge.h"


using namespace FuseeQtHostBridge;

QtApplication::QtApplication(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::QtApplication)
{
    ui->setupUi(this);
	_pQH = NULL;
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
	_pQH = new FuseeWidget(this->ui->renderParent);
	ui->pushButton->setEnabled(false);
}

void QtApplication::SliderChanged()
{
	if (_pQH)
	{
		int color = ui->redSlider->value() << 16 | ui->greenSlider->value() << 8 | ui->blueSlider->value();
		_pQH->SetTeapotColor(color);
	}
}

void QtApplication::on_redSlider_valueChanged()
{
	SliderChanged();
}

void QtApplication::on_greenSlider_valueChanged()
{
	SliderChanged();
}

void QtApplication::on_blueSlider_valueChanged()
{
	SliderChanged();
}
