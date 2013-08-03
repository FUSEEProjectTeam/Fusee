#include "FuseeWidget.h"
#include <qevent.h>
#include <qwidget.h>
#include <qlayout.h>
#include <qtimer.h>

FuseeWidget::FuseeWidget(QWidget *pParent) 
	: QWidget(pParent)
{
	_pBridge = new QtHostBridge((void *)this->winId());
	_pBridge->SetMousePosQuery(FuseeWidget::GetCursorPos);

	QTimer *timer = new QTimer(this);
    connect(timer, SIGNAL(timeout()), this, SLOT(OnIdle()));
    timer->start(0);

	pParent->layout()->addWidget(this);
	this->setSizePolicy(QSizePolicy(QSizePolicy::Policy::Preferred, QSizePolicy::Policy::Preferred));
	this->show();
}


FuseeWidget::~FuseeWidget(void)
{
}


int FuseeWidget::GetCursorPos()
{
	QPoint pt = QCursor::pos();
	return ((pt.x() & 0xFFFF) << 16) | (pt.y() & 0xFFFF);
}

void FuseeWidget::mousePressEvent(QMouseEvent * evt)
{
	_pBridge->TriggerMouseDown(evt->button() & (1|2|4), evt->x(), evt->y());
}

void FuseeWidget::mouseReleaseEvent(QMouseEvent * evt)
{
	_pBridge->TriggerMouseUp(evt->button() & (1|2|4), evt->x(), evt->y());
}

void FuseeWidget::keyPressEvent(QKeyEvent *evt)
{
	_pBridge->TriggerKeyDown(evt->nativeVirtualKey()); 
}

void FuseeWidget::keyReleaseEvent(QKeyEvent *evt)
{
	_pBridge->TriggerKeyDown(evt->nativeVirtualKey()); 	
}

void FuseeWidget::resizeEvent(QResizeEvent *evt)
{
	_pBridge->TriggerSizeChanged(evt->size().width(), evt->size().height());
}

void FuseeWidget::OnIdle()
{
	_pBridge->TriggerIdle();
}

void FuseeWidget::SetTeapotColor(int color)
{
	_pBridge->SetTeapotColor(color);
}