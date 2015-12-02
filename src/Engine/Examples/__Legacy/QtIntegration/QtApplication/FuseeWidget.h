#pragma once
#include "qwidget.h"
#include "QtHostBridge.h"

using namespace FuseeQtHostBridge;


class FuseeWidget : QWidget
{
    Q_OBJECT

protected slots:
	void OnIdle();
protected:
	QtHostBridge *_pBridge;

	// virtual void	mouseDoubleClickEvent(QMouseEvent * evt);
	// virtual void	mouseMoveEvent(QMouseEvent * evt);
	virtual void mousePressEvent(QMouseEvent * evt);
	virtual void mouseReleaseEvent(QMouseEvent * evt);
    virtual void keyPressEvent(QKeyEvent *evt);
    virtual void keyReleaseEvent(QKeyEvent *evt);
	virtual void resizeEvent(QResizeEvent *evt);

	static int GetCursorPos();
public:
	FuseeWidget(QWidget *pWidget);
	~FuseeWidget(void);

	void SetTeapotColor(int color);
};