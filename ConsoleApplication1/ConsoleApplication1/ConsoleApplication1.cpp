#include <stdafx.h>
#include <stdlib.h>
#include <GL/glut.h> 
#include <iostream>

using namespace std;

void background(void)
{
	glClearColor(0.0, 0.0, 0.0, 0.0);//设置背景颜色为黑色
}

void myDisplay(void)
{
	glClear(GL_COLOR_BUFFER_BIT);//buffer设置为颜色可写

	glBegin(GL_TRIANGLES);//开始画三角形
	glShadeModel(GL_SMOOTH);//设置为光滑明暗模式
	
	glColor3f(1.0, 0.0, 0.0);//设置第一个顶点为红色
	glVertex2f(-1.0, -1.0);//设置第一个顶点的坐标为（-1.0，-1.0）

	glColor3f(0.0, 1.0, 0.0);//设置第二个顶点为绿色
	glVertex2f(0.0, -1.0);//设置第二个顶点的坐标为（0.0，-1.0）

	glColor3f(0.0, 0.0, 1.0);//设置第三个顶点为蓝色
	glVertex2f(-0.5, 1.0);//设置第三个顶点的坐标为（-0.5，1.0）
	glEnd();//三角形结束

	glFlush();//强制OpenGL函数在有限时间内运行
}

void myReshape(GLsizei w, GLsizei h)
{
	glViewport(100, 0, w, h);//设置视口，前两个参数设置左下角的位置

	glMatrixMode(GL_PROJECTION);//指明当前矩阵为GL_PROJECTION
	glLoadIdentity();//将当前矩阵置换为单位阵
	cout << w << " " << h << endl;
	if (w <= h)
		gluOrtho2D(-1.0, 1.5, -1.5, 1.5*(GLfloat)h / (GLfloat)w);//定义二维正视投影矩阵
	else
		gluOrtho2D(-1.0, 1.5*(GLfloat)w / (GLfloat)h, -1.5, 1.5);
	glMatrixMode(GL_MODELVIEW);//指明当前矩阵为GL_MODELVIEW
}

int main(int argc, char ** argv)
{
	/*初始化*/
	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_SINGLE | GLUT_RGB);//缺省模式
	glutInitWindowSize(500, 400);//设置窗口的尺寸
	glutInitWindowPosition(200, 200);//设置窗口的位置

	/*创建窗口*/
	glutCreateWindow("Triangle");//创建顶层窗口，定义窗口的名字

	/*绘制与显示*/
	background();//设置背景
	glutReshapeFunc(myReshape);
	glutDisplayFunc(myDisplay);
	glutMainLoop();
	return(0);
}