#ifndef APPBROWSERWND_H
#define APPBROWSERWND_H

#include <QMainWindow>
#include "FuseeWidget.h"

namespace Ui {
class QtApplication;
}

class QtApplication : public QMainWindow
{
    Q_OBJECT
    
public:
    explicit QtApplication(QWidget *parent = 0);
    ~QtApplication();
    
public slots:
	void on_pushButton_clicked();
	void on_redSlider_valueChanged();
	void on_greenSlider_valueChanged();
	void on_blueSlider_valueChanged();

private:
	FuseeWidget *_pQH;
	void SliderChanged();
    Ui::QtApplication *ui;
};

#endif // APPBROWSERWND_H
