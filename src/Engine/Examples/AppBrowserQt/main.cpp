#include "appbrowserwnd.h"
#include <QApplication>

int main(int argc, char *argv[])
{
    QApplication a(argc, argv);
	a.setAttribute(Qt::AA_NativeWindows);

	AppBrowserWnd w;
	w.setAttribute(Qt::WA_NativeWindow );
	w.show();
    
    return a.exec();
}
