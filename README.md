# Unity-3D_Marker
3D Marker for Unity UI Canvas

Маркеры на объекты в 3D для стандартного UI Canvas. Маркеры имеют тонкую настройку могу менять размер в зависимости от растояния до объекта или в зависимости от направления взгляда. Можно управлять положением относительно цели. Так же можно добавить стрелочки указывающие на объект. 
![5c595e0e6c71313036614987a262526](https://user-images.githubusercontent.com/99481254/183256827-041eef0a-c08c-486b-9103-8845eb3621bb.png)

Проект состоит из: <br>
MarkerUI - основной компонент, необходимо вешать на UI объекты которые хотите сделать маркерами. Имеет настройки положения относительно указываемого объекта и изменения маштаба. Так же для более тонкой настройки поведения имеет два свобытия OnTargetEnter - вызывается кода цель находится в передлах экрана и OnTargetExit когда цель вне экрана.

![8f3fe18c1e36cefb18217f9d26a1f6e](https://user-images.githubusercontent.com/99481254/183256798-a4ecd412-21bb-4482-ba86-9240d8ca4d72.png)

MarkerArrowUI - стрелочка указывающая направление. Повесте компонент MarkerArrowUI на дочерний для MarkerUI объект, и он сам найдет MarkerUI в связке с которым будет работать. 

![6d4c8f8d766ca80eaf33244c6b48add](https://user-images.githubusercontent.com/99481254/183256793-c99cc473-d976-4162-a852-7e4f4062f380.png)

MarkerGroupUI - группирует все дочерние объекты с компонентами MarkerUI. Испольузется для сортировки порядка отрисовки маркеров в зависимости от растояния.

![86f2e6a9dc5b13a5c6b13c1f9c4eb01](https://user-images.githubusercontent.com/99481254/183256788-57b7c5d3-2287-4da9-80fa-c1a2d27abb1b.png)

Warning! Проект находится в стадии разработки, его API может меняться 
