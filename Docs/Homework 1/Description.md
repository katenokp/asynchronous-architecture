### Event storming
![EventStorming.svg](EventStorming.svg)

#### Спорные моменты
- Не понятно, есть ли в аналитике бизнес-события. Кажется, для аналитики только CUD нужны
- Нужна ли вообще аналитика отдельно, тоже вопрос. Сейчас там в общем-то то же самое, что и в аккаунтинге, но под другим углом. Кажется, лучше всё-таки отдельно, потому что там другая терминология, и не сложно представить что бизнес захочет считать количество созданных за день тасок, или время от создания таски до закрытия. С другой стороны, это какие-то совершенно другие метрики, и в один домен они с виду плохо запихиваются. Оставила пока что отдельно, но кажется это какая-то подстава
- Объединила команды изменения баланса в одну. Для аккаунтинга это, кажется, одно событие, раз физические начисления отдельно происходят. А если кому-то дальше (аналитике, например) это важно, пусть сами разбираются. А ещё до объединения не получалось коротко назвать команды: когда таска завершена, это ещё не начисление, само начисление позже будет
- Непонятно, нужно ли отдельное событие про сброс баланса в конце дня. Кажется, это то же изменение баланса, но у него другая природа

### Data model
![DataModel](DataModel.svg)


#### Спорные моменты
- Нарисованы data stream с информацией о юзерах, но если с юзерами ничего не может происходить, они не нужны. Если может, то сервис аналитики тоже должен их ловить, наверное
- Пока непонятно, могут ли ещё какие-то важные аналитике изменения происходить с payment и account помимо тех, что в бизнес-событиях отражены

### Определяем коммуникации
![Communications.svg](Communications.svg)

#### Спорные моменты
- Непонятно, что умеет UberPopug Inc
- Теоретически нам ещё нужна информация про то, что что-то случилось с пользователем. Роль изменилась, например. Если что-то подобное возможно в нашей системе, нужны CUD события от UberPopug Inc
- Пока непонятно, хватит ли сервису аналитики уже существующих событий. Скорее всего нет, недостающее аккаунтинг будет отправлять через CUD события