#ifndef APPBROWSERWND_H
#define APPBROWSERWND_H

#include <QMainWindow>

namespace Ui {
class AppBrowserWnd;
}

class AppBrowserWnd : public QMainWindow
{
    Q_OBJECT
    
public:
    explicit AppBrowserWnd(QWidget *parent = 0);
    ~AppBrowserWnd();
    
public slots:
	void on_pushButton_clicked();

private:
    Ui::AppBrowserWnd *ui;
};

#endif // APPBROWSERWND_H
