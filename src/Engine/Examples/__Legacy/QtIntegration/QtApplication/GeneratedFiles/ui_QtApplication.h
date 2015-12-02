/********************************************************************************
** Form generated from reading UI file 'QtApplication.ui'
**
** Created by: Qt User Interface Compiler version 5.0.2
**
** WARNING! All changes made in this file will be lost when recompiling UI file!
********************************************************************************/

#ifndef UI_QTAPPLICATION_H
#define UI_QTAPPLICATION_H

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
#include <QtWidgets/QSlider>
#include <QtWidgets/QStatusBar>
#include <QtWidgets/QToolBar>
#include <QtWidgets/QVBoxLayout>
#include <QtWidgets/QWidget>

QT_BEGIN_NAMESPACE

class Ui_QtApplication
{
public:
    QWidget *centralWidget;
    QHBoxLayout *horizontalLayout;
    QFrame *frame;
    QVBoxLayout *verticalLayout;
    QSlider *redSlider;
    QSlider *greenSlider;
    QSlider *blueSlider;
    QPushButton *pushButton;
    QWidget *renderParent;
    QVBoxLayout *verticalLayout_2;
    QMenuBar *menuBar;
    QToolBar *mainToolBar;
    QStatusBar *statusBar;

    void setupUi(QMainWindow *QtApplication)
    {
        if (QtApplication->objectName().isEmpty())
            QtApplication->setObjectName(QStringLiteral("QtApplication"));
        QtApplication->resize(900, 603);
        centralWidget = new QWidget(QtApplication);
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
        redSlider = new QSlider(frame);
        redSlider->setObjectName(QStringLiteral("redSlider"));
        redSlider->setMaximum(255);
        redSlider->setOrientation(Qt::Horizontal);
        redSlider->setInvertedControls(false);

        verticalLayout->addWidget(redSlider);

        greenSlider = new QSlider(frame);
        greenSlider->setObjectName(QStringLiteral("greenSlider"));
        greenSlider->setMaximum(255);
        greenSlider->setOrientation(Qt::Horizontal);

        verticalLayout->addWidget(greenSlider);

        blueSlider = new QSlider(frame);
        blueSlider->setObjectName(QStringLiteral("blueSlider"));
        blueSlider->setMaximum(255);
        blueSlider->setOrientation(Qt::Horizontal);

        verticalLayout->addWidget(blueSlider);

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

        QtApplication->setCentralWidget(centralWidget);
        menuBar = new QMenuBar(QtApplication);
        menuBar->setObjectName(QStringLiteral("menuBar"));
        menuBar->setGeometry(QRect(0, 0, 900, 23));
        QtApplication->setMenuBar(menuBar);
        mainToolBar = new QToolBar(QtApplication);
        mainToolBar->setObjectName(QStringLiteral("mainToolBar"));
        QtApplication->addToolBar(Qt::TopToolBarArea, mainToolBar);
        statusBar = new QStatusBar(QtApplication);
        statusBar->setObjectName(QStringLiteral("statusBar"));
        QtApplication->setStatusBar(statusBar);

        retranslateUi(QtApplication);

        QMetaObject::connectSlotsByName(QtApplication);
    } // setupUi

    void retranslateUi(QMainWindow *QtApplication)
    {
        QtApplication->setWindowTitle(QApplication::translate("QtApplication", "QtApplication", 0));
        pushButton->setText(QApplication::translate("QtApplication", "Start FUSEE App", 0));
    } // retranslateUi

};

namespace Ui {
    class QtApplication: public Ui_QtApplication {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_QTAPPLICATION_H
