#include <stdafx.h>
#include <stdlib.h>
#include <GL/glut.h> 
#include <iostream>

using namespace std;

void background(void)
{
	glClearColor(0.0, 0.0, 0.0, 0.0);//���ñ�����ɫΪ��ɫ
}

void myDisplay(void)
{
	glClear(GL_COLOR_BUFFER_BIT);//buffer����Ϊ��ɫ��д

	glBegin(GL_TRIANGLES);//��ʼ��������
	glShadeModel(GL_SMOOTH);//����Ϊ�⻬����ģʽ
	
	glColor3f(1.0, 0.0, 0.0);//���õ�һ������Ϊ��ɫ
	glVertex2f(-1.0, -1.0);//���õ�һ�����������Ϊ��-1.0��-1.0��

	glColor3f(0.0, 1.0, 0.0);//���õڶ�������Ϊ��ɫ
	glVertex2f(0.0, -1.0);//���õڶ������������Ϊ��0.0��-1.0��

	glColor3f(0.0, 0.0, 1.0);//���õ���������Ϊ��ɫ
	glVertex2f(-0.5, 1.0);//���õ��������������Ϊ��-0.5��1.0��
	glEnd();//�����ν���

	glFlush();//ǿ��OpenGL����������ʱ��������
}

void myReshape(GLsizei w, GLsizei h)
{
	glViewport(100, 0, w, h);//�����ӿڣ�ǰ���������������½ǵ�λ��

	glMatrixMode(GL_PROJECTION);//ָ����ǰ����ΪGL_PROJECTION
	glLoadIdentity();//����ǰ�����û�Ϊ��λ��
	cout << w << " " << h << endl;
	if (w <= h)
		gluOrtho2D(-1.0, 1.5, -1.5, 1.5*(GLfloat)h / (GLfloat)w);//�����ά����ͶӰ����
	else
		gluOrtho2D(-1.0, 1.5*(GLfloat)w / (GLfloat)h, -1.5, 1.5);
	glMatrixMode(GL_MODELVIEW);//ָ����ǰ����ΪGL_MODELVIEW
}

int main(int argc, char ** argv)
{
	/*��ʼ��*/
	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_SINGLE | GLUT_RGB);//ȱʡģʽ
	glutInitWindowSize(500, 400);//���ô��ڵĳߴ�
	glutInitWindowPosition(200, 200);//���ô��ڵ�λ��

	/*��������*/
	glutCreateWindow("Triangle");//�������㴰�ڣ����崰�ڵ�����

	/*��������ʾ*/
	background();//���ñ���
	glutReshapeFunc(myReshape);
	glutDisplayFunc(myDisplay);
	glutMainLoop();
	return(0);
}