#ifndef APPBROWSERWND_H
#define APPBROWSERWND_H

#include <QMainWindow>

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

private:
    Ui::QtApplication *ui;
};

#endif // APPBROWSERWND_H
