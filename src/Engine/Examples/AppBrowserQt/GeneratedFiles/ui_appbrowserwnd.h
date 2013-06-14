/********************************************************************************
** Form generated from reading UI file 'appbrowserwnd.ui'
**
** Created by: Qt User Interface Compiler version 5.0.2
**
** WARNING! All changes made in this file will be lost when recompiling UI file!
********************************************************************************/

#ifndef UI_APPBROWSERWND_H
#define UI_APPBROWSERWND_H

#include <QtCore/QVariant>
#include <QtWidgets/QAction>
#include <QtWidgets/QApplication>
#include <QtWidgets/QButtonGroup>
#include <QtWidgets/QFrame>
#include <QtWidgets/QHBoxLayout>
#include <QtWidgets/QHeaderView>
#include <QtWidgets/QMainWindow>
#include <QtWidgets/QMenuBar>
#include <QtWidgets/QPushButton>
#include <QtWidgets/QStatusBar>
#include <QtWidgets/QToolBar>
#include <QtWidgets/QVBoxLayout>
#include <QtWidgets/QWidget>

QT_BEGIN_NAMESPACE

class Ui_AppBrowserWnd
{
public:
    QWidget *centralWidget;
    QHBoxLayout *horizontalLayout;
    QFrame *frame;
    QVBoxLayout *verticalLayout;
    QPushButton *pushButton;
    QWidget *renderParent;
    QVBoxLayout *verticalLayout_2;
    QMenuBar *menuBar;
    QToolBar *mainToolBar;
    QStatusBar *statusBar;

    void setupUi(QMainWindow *AppBrowserWnd)
    {
        if (AppBrowserWnd->objectName().isEmpty())
            AppBrowserWnd->setObjectName(QStringLiteral("AppBrowserWnd"));
        AppBrowserWnd->resize(900, 818);
        centralWidget = new QWidget(AppBrowserWnd);
        centralWidget->setObjectName(QStringLiteral("centralWidget"));
        horizontalLayout = new QHBoxLayout(centralWidget);
        horizontalLayout->setSpacing(6);
        horizontalLayout->setContentsMargins(11, 11, 11, 11);
        horizontalLayout->setObjectName(QStringLiteral("horizontalLayout"));
        frame = new QFrame(centralWidget);
        frame->setObjectName(QStringLiteral("frame"));
        QSizePolicy sizePolicy(QSizePolicy::Fixed, QSizePolicy::Preferred);
        sizePolicy.setHorizontalStretch(0);
        sizePolicy.setVerticalStretch(0);
        sizePolicy.setHeightForWidth(frame->sizePolicy().hasHeightForWidth());
        frame->setSizePolicy(sizePolicy);
        frame->setMinimumSize(QSize(150, 0));
        frame->setFrameShape(QFrame::StyledPanel);
        frame->setFrameShadow(QFrame::Raised);
        verticalLayout = new QVBoxLayout(frame);
        verticalLayout->setSpacing(6);
        verticalLayout->setContentsMargins(11, 11, 11, 11);
        verticalLayout->setObjectName(QStringLiteral("verticalLayout"));
        pushButton = new QPushButton(frame);
        pushButton->setObjectName(QStringLiteral("pushButton"));

        verticalLayout->addWidget(pushButton);


        horizontalLayout->addWidget(frame);

        renderParent = new QWidget(centralWidget);
        renderParent->setObjectName(QStringLiteral("renderParent"));
        verticalLayout_2 = new QVBoxLayout(renderParent);
        verticalLayout_2->setSpacing(6);
        verticalLayout_2->setContentsMargins(11, 11, 11, 11);
        verticalLayout_2->setObjectName(QStringLiteral("verticalLayout_2"));

        horizontalLayout->addWidget(renderParent);

        AppBrowserWnd->setCentralWidget(centralWidget);
        menuBar = new QMenuBar(AppBrowserWnd);
        menuBar->setObjectName(QStringLiteral("menuBar"));
        menuBar->setGeometry(QRect(0, 0, 900, 23));
        AppBrowserWnd->setMenuBar(menuBar);
        mainToolBar = new QToolBar(AppBrowserWnd);
        mainToolBar->setObjectName(QStringLiteral("mainToolBar"));
        AppBrowserWnd->addToolBar(Qt::TopToolBarArea, mainToolBar);
        statusBar = new QStatusBar(AppBrowserWnd);
        statusBar->setObjectName(QStringLiteral("statusBar"));
        AppBrowserWnd->setStatusBar(statusBar);

        retranslateUi(AppBrowserWnd);

        QMetaObject::connectSlotsByName(AppBrowserWnd);
    } // setupUi

    void retranslateUi(QMainWindow *AppBrowserWnd)
    {
        AppBrowserWnd->setWindowTitle(QApplication::translate("AppBrowserWnd", "AppBrowserWnd", 0));
        pushButton->setText(QApplication::translate("AppBrowserWnd", "Dr\303\274ckmich", 0));
    } // retranslateUi

};

namespace Ui {
    class AppBrowserWnd: public Ui_AppBrowserWnd {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_APPBROWSERWND_H
