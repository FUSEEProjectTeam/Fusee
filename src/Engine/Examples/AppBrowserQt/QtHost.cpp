#include "QtHost.h"
#include <qevent.h>
#include <qwidget.h>
#include <qlayout.h>
#include <qtimer.h>

QtHost::QtHost(QWidget *pParent) 
	: QWidget(pParent)
{
	_pBridge = new QtHostBridge((void *)this->winId());
	_pBridge->SetMousePosQuery(QtHost::GetCursorPos);
	
	QTimer *timer = new QTimer(this);
    connect(timer, SIGNAL(timeout()), this, SLOT(OnIdle()));
    timer->start(1000);

	pParent->layout()->addWidget(this);
	this->setSizePolicy(QSizePolicy(QSizePolicy::Policy::Preferred, QSizePolicy::Policy::Preferred));
	this->show();
}


QtHost::~QtHost(void)
{
}

/*
QCursor::pos() 


void QtHost::mouseMoveEvent(QMouseEvent * evt)
{
	_pBridge->T
}
*/

int QtHost::GetCursorPos()
{
	QPoint pt = QCursor::pos();
	return ((pt.x() & 0xFFFF) << 16) | (pt.y() & 0xFFFF);
}

void QtHost::mousePressEvent(QMouseEvent * evt)
{
	_pBridge->TriggerMouseDown(evt->buttons() & (1|2|4), evt->x(), evt->y());
}

void QtHost::mouseReleaseEvent(QMouseEvent * evt)
{
	_pBridge->TriggerMouseUp(evt->buttons() & (1|2|4), evt->x(), evt->y());
}

void QtHost::keyPressEvent(QKeyEvent *evt)
{
	_pBridge->TriggerKeyDown(evt->nativeVirtualKey()); 
}

void QtHost::keyReleaseEvent(QKeyEvent *evt)
{
	_pBridge->TriggerKeyDown(evt->nativeVirtualKey()); 	
}

void QtHost::resizeEvent(QResizeEvent *evt)
{
	_pBridge->TriggerSizeChanged(evt->size().width(), evt->size().height());
}

void QtHost::OnIdle()
{
	_pBridge->TriggerIdle();
}