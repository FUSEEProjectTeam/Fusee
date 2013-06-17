#include "QtApplication.h"
#include <QApplication>

int main(int argc, char *argv[])
{
    QApplication a(argc, argv);
	a.setAttribute(Qt::AA_NativeWindows);

	QtApplication w;
	w.setAttribute(Qt::WA_NativeWindow );
	w.show();
    
    return a.exec();
}
