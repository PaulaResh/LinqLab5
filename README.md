### Решетникова Полина ЛА-1 Лабораторная №5
# ТЕМА: LINQ-запросы. БД «Детские товары», вариант 1.

# ОПИСАНИЕ ПРЕДМЕТНОЙ ОБЛАСТИ
Исходный файл LR5-var1.xls, четыре таблицы:
- «Движение товаров» (8375 записей): ID операции, дата,
  ID магазина, артикул, тип операции
  (Поступление/Продажа/Возврат), количество упаковок,
  наличие карты клиента (Да/Нет/-);
- «Товар» (74): артикул, ID категории, наименование,
  цена закупки, цена продажи без скидки,
  скидка при наличии карты, %;
- «Категория» (18): ID, наименование,
  возрастное ограничение;
- «Магазин» (15): ID (Р1..Р15), район, адрес.
Связи: движение.ID магазина → магазин,
движение.Артикул → товар,
товар.ID категории → категория.

# КЛАССЫ ПРОГРАММЫ
Модели (свойства, конструктор, перегруженный ToString):
- Category: Id, Name, AgeLimit;
- Store: Id (string, формат РN), District, Address;
- Product: Article, CategoryId, Name, PurchasePrice,
  SalePrice, DiscountPercent; метод PriceWithCard —
  цена продажи с учётом скидки по карте;
- Movement: OperationId, Date (DateOnly), StoreId,
  Article, Type (enum OperationType), PackageCount,
  HasClientCard (bool?, null означает «-»);
- OperationType: Receipt, Sale, Return.
Вспомогательные классы (весь функционал — методы
с LINQ):
- Database — CRUD с проверками ключей и внешних
  ключей (Any, FirstOrDefault, Count), событие Changed;
- Queries — четыре запроса задания 5;
- ExcelStorage — чтение и запись Excel (NPOI);
- ConsoleHelper — ввод с проверкой, меню;
- AppExtensions — расширяющие методы
  (ToMoney, ToRu, ToText, ToCardText, ToOperationType);
- Program — меню с выбором номера задания 1–6,
  выход только по специальному слову exit (q/выход).

# ЗАДАНИЕ 1.
## ТЕКСТ ЗАДАЧИ:
Чтение базы данных из Excel-файла LR5-var1.xls
в объекты программы.
## АЛГОРИТМ РЕШЕНИЯ:
ExcelStorage.Load проверяет наличие файла
(File.Exists, иначе FileNotFoundException).
Файл открывает NPOI: WorkbookFactory.Create сам
определяет формат .xls/.xlsx.
Обобщённый метод ReadRows<T> обходит строки листа
со второй; делегат Func<IRow, T> создаёт объект.
Пустые строки пропускаются через IsEmptyRow —
в конце листа Excel есть такие строки.
Разбор ячеек — методы GetInt, GetDecimal, GetDate,
GetCard, GetPercent: проверка типа ячейки и формата;
при ошибке — FormatException с именем листа и номером
строки, который ReadRows превращает
в InvalidDataException с тем же контекстом.
## ТЕСТИРОВАНИЕ
Ввод: задание 1, на вопрос о файле — Enter.
Ожидание: «Категорий: 18, магазинов: 15,
товаров: 74, движений: 8375.»
Ввод: задание 1, файл nofile.xls.
Ожидание: «Ошибка: Файл не найден.»

![](https://sun9-72.vkuserphoto.ru/s/v1/ig2/raDO9MHkHDBukBbwDOayt08UWr2qxXj63eMsjTwf8oTY6YSY9LXWCezTVqZ1TbGNXYAnlwebqhqi9KA2d9Vpank-.jpg?quality=95&as=32x13,48x19,72x29,108x44,160x65,240x97,360x145,478x193&from=bu&u=vGqaqh84lN7DmLztVNDM19n_VTYCJljnKG4PGReQXu4&cs=478x0)
![](https://sun9-45.vkuserphoto.ru/s/v1/ig2/q0IlgpSZbmpskKN2Hl0avcAb9qJ9tuUOrhGl6Lqq5JPVykgLjlcLR6lSViM9IZlEpbUahJPNoVmQELWzoSETJD2Y.jpg?quality=95&as=32x18,48x28,72x41,108x62,160x92,240x138,333x191&from=bu&u=sVEf0KnUo2UghpF7rh-NSxf8PqWl56x3t9AM5o8xDlA&cs=333x0)


# ЗАДАНИЕ 2.
## ТЕКСТ ЗАДАЧИ:
Просмотр базы данных: любая из четырёх таблиц
или все сразу.
## АЛГОРИТМ РЕШЕНИЯ:
Пункт меню 2, выбор таблицы (1–4, 5 — все).
Вывод через LINQ: db.Categories.Select(x => x.ToString())
и аналогично для остальных таблиц; результат передаётся
в ConsoleHelper.PrintLines.
PrintLines печатает с постраничной разбивкой по 20
строк (Enter — далее) и выводит счётчик записей.
Формат строки задаёт перегруженный ToString модели.
## ТЕСТИРОВАНИЕ
Ввод: 2 → 3. Ожидание: таблица «Товар»,
в конце «Всего записей: 74».
Ввод: 2 → 4. Ожидание: «Движение товаров»
страницами по 20, приглашение «-- Enter: далее --»,
в конце «Всего записей: 8375».
Ввод: 2 → 0. Ожидание: возврат в главное меню.

![](https://sun9-62.vkuserphoto.ru/s/v1/ig2/Pc1f139KxsHJLc5HrIy3MgRif1TTMeLW0LTN4lCPlfdTqVvCxQXaZViVV4CzRrbkAtsMq_xWSbw_P4N_sNiKtFNw.jpg?quality=95&as=32x20,48x29,72x44,108x66,160x98,240x147,360x220,480x294,540x331,640x392,720x441,934x572&from=bu&u=fKubRoLFUN8siWFjg8xmHGuRo5Cr9V40dLsued1geiQ&cs=934x0)
![](https://sun9-37.vkuserphoto.ru/s/v1/ig2/ASiqmo3-OyNKQFQ5hx3l6Ms8UHRVKZhBhGsYFwsUJvWEI_VQOdoT9bBgzBbprP1U96G12nNFbGfQ1QllIdObLwE1.jpg?quality=95&as=32x27,48x40,72x60,108x90,160x133,240x200,331x276&from=bu&cs=331x0)
![](https://sun9-46.vkuserphoto.ru/s/v1/ig2/P3SMJKhfLajMBnN_0PaiURKV3BE30qkAzkDGCPqK9IAdOClEw3Xac7mE58cEP3ej4z0knM0w3Naydqs7A6yeISA2.jpg?quality=95&as=32x41,48x61,72x92,108x137,160x203,240x305,360x458,439x558&from=bu&cs=439x0)
![](https://sun9-7.vkuserphoto.ru/s/v1/ig2/P_-xVj6-4tgJ1jJa4hVhUtxEC8L7iNP8QLI3XcNDfmCvdCkzOqSnQl6EIr-tiRewpMNnNh0fQpIgvTStYcSpUjKjXn6Hxg.jpg?quality=95&as=32x20,48x31,72x46,108x69,160x102,240x153,360x229,467x297&from=bu&u=cnokMA3bQ7743IXi9t_J5byvbu0_IcJYBq5AVattPIs&cs=467x0)
![](https://sun9-2.vkuserphoto.ru/s/v1/ig2/J8IsLz7ZTea8gBEL7zgUmpHHuJJ8qKb8ICUfnYxlFvOT0IQO7u3G-pmrE0_AcMQ4X0AUjbL5DVKoP1s1rAOFBMNX.jpg?quality=95&as=32x26,48x39,72x59,108x88,160x131,240x196,278x227&from=bu&cs=278x0)


# ЗАДАНИЕ 3.
## ТЕКСТ ЗАДАЧИ:
Удаление элементов по ключу с контролем
целостности связей.
## АЛГОРИТМ РЕШЕНИЯ:
Пункт меню 3: выбор таблицы и ввод ключа.
В Database элемент ищется через FirstOrDefault;
если null — KeyNotFoundException с текстом ключа.
Перед удалением через Count проверяются зависимые
записи: у категории — товары, у магазина и товара —
операции движения. Если Count > 0 —
InvalidOperationException, удаление запрещено.
Иначе Remove и вызов события Changed.
## ТЕСТИРОВАНИЕ
Ввод: 3 → 4 → ID 9001 (предварительно добавлен
в задании 4). Ожидание: «[БД] Удалена операция 9001.»
Ввод: 3 → 4 → 999999. Ожидание: «Ошибка: Операция
999999 не найдена.»
Ввод: 3 → 2 → Р3. Ожидание: отказ «Есть операции
этого магазина: …» со счётчиком.
Ввод: 3 → 1 → 3. Ожидание: «Ошибка: Есть товары
этой категории: 5.»

![](https://sun9-51.vkuserphoto.ru/s/v1/ig2/wlEI6m471iAlJ9KVT-EcAhBF56rG1NmxIw5A6FWKiC1bGWP9SSvMNPatfvviZ8r5WASkRCz4NfAf1q-4rgKdXC1c.jpg?quality=95&as=32x18,48x27,72x41,108x61,160x91,240x136,360x204,391x222&from=bu&u=ycDTZumtCV-K6Yh3EM3oKpGDuAKGxgBdQmj_018x98M&cs=391x0)
![](https://sun9-56.vkuserphoto.ru/s/v1/ig2/08pcBAs-H0i22Io3v68xX4lcEQHaFi0y8DuVPN98ccSsHO0OwwArONZhSFC52_rrxS-E0sWZYptb3PdSX0wSJLb3.jpg?quality=95&as=32x21,48x31,72x46,108x69,160x103,240x154,360x231,392x252&from=bu&cs=392x0)
![](https://sun9-2.vkuserphoto.ru/s/v1/ig2/zjQCznm0VyWfnyvvhpdxRnc565i_o27DLVNtEbpmIIkuPQzcwg4QVql-w3BHZ6pPUHsrmm_vf7sB014y-K17nP0b.jpg?quality=95&as=32x18,48x26,72x40,108x59,160x88,240x132,360x198,395x217&from=bu&cs=395x0)
![](https://sun9-56.vkuserphoto.ru/s/v1/ig2/1N6MclknyesAuX5s-KM1gyk0f6Grips_mAErjYza-umwDMY0lW1rNJq5wEHKCvoQPkpXpuCyEdiYJdOX2JPZu47-.jpg?quality=95&as=32x51,48x76,72x115,108x172,160x255,240x382,324x516&from=bu&cs=324x0)

# ЗАДАНИЕ 4.
## ТЕКСТ ЗАДАЧИ:
Добавление элементов во все четыре таблицы
с проверкой входных данных.
## АЛГОРИТМ РЕШЕНИЯ:
Пункт меню 4: выбор таблицы, затем ввод полей через
ConsoleHelper (проверки — раздел «ВАЛИДАЦИЯ»).
Методы Database.Add проверяют: уникальность ключа
(Any), существование внешних ключей (Any),
цены >= 0, скидку 0..100, количество > 0.
Правило карты: у поступления HasClientCard = null
(в файле «-»), у продажи и возврата — обязательно
Да или Нет.
При успехе — Add в коллекцию и событие Changed.
## ТЕСТИРОВАНИЕ
Ввод: 4 → 2 → Р99, «Тестовый», «ул. Тест, 1».
Ожидание: «[БД] Добавлен магазин Р99.»
Ввод: 4 → 2 → Р1. Ожидание: «Ошибка: Магазин Р1
уже существует.»
Ввод: 4 → 2 → ID «1». Ожидание: «Ошибка: ID вида
Р1, Р15.» (повторный запрос ввода)
Ввод: 4 → 3 → артикул «12345a». Ожидание: «Ошибка:
артикул: только цифры».
Ввод: 4 → 3 → категория 99. Ожидание: «Ошибка:
Категория 99 не найдена.»
Ввод: 4 → 3 → скидка 150. Ожидание: «Ошибка: нужно
целое 0..100.»
Ввод: 4 → 4 → магазин Р90. Ожидание: «Ошибка:
Магазин Р90 не найден.»
Ввод: 4 → 4 → количество 0. Ожидание: «Ошибка:
Количество должно быть больше нуля.»
Ввод: 4 → 4 → дата 31.02.2024. Ожидание: «Ошибка:
формат дд.мм.гггг.»
Ввод: 4 → 4 → 9001, 25.08.2024, Р3, 360557, тип 2,
5, Да. Ожидание: «[БД] Добавлена операция 9001.»

![](https://sun9-70.vkuserphoto.ru/s/v1/ig2/yo3RWVezuBW1l7D9nyqIF34NKgdGTqYW57Gw1s5tyTfYCJ5VIUccE_mLssNWOAQPm5jeB4_QjHF_y5yCn0IMT9hK6z2rOw.jpg?quality=95&as=32x20,48x31,72x46,108x69,160x102,240x153,360x230,390x249&from=bu&u=_eYsfC56UOarvp8VOfLF2hLrw_jDCt3XRZqtcsBhqKU&cs=390x0)
![](https://sun9-62.vkuserphoto.ru/s/v1/ig2/8VrV_HW9uZgtqrWnvr3NOU_iRHPYsEQ-A3q0_eRy0WdZlf4hvvaPBiROV0CRCT7ZRUzK5vEk8s1LJYKMXLZ0Zs4l.jpg?quality=95&as=32x21,48x32,72x48,108x71,160x106,240x159,360x238,387x256&from=bu&cs=387x0)
![](https://sun9-46.vkuserphoto.ru/s/v1/ig2/G95C4WoNL2PNR_IJSJxHId5TqdyTmtrUW-AT_JFWwVULw41OsYOH3FCkETVoIhfpgmI-XVDNzqmPxT5p7PG8y_7I.jpg?quality=95&as=32x20,48x30,72x45,108x68,160x100,240x151,360x226,387x243&from=bu&cs=387x0)
![](https://sun9-63.vkuserphoto.ru/s/v1/ig2/yJdJigJgyLgiYmi0RMzYmv6M-RaY4twwlIMPqCEgncdP6s2_9qi1y47RvqCCcQghlORKgjniKOiRDQpNtmJ9R1H4.jpg?quality=95&as=32x18,48x27,72x40,108x60,160x89,240x133,360x200,392x218&from=bu&cs=392x0)
![](https://sun9-82.vkuserphoto.ru/s/v1/ig2/SljCVfr4x3Fqijc3FD_qfJCfdM_8WK2Kj4zMoApa7VrHRAMUBm3445YzzCKUALSelnfvHsslD554j1T22yLMaxVB.jpg?quality=95&as=32x42,48x63,72x95,108x143,160x211,240x317,360x475,403x532&from=bu&cs=403x0)
![](https://sun9-16.vkuserphoto.ru/s/v1/ig2/NAVgRYv9rjz28j45TQSN4pcELKKNL2FIO4uZeydij-C49JU4aQJch8bJuE8koZc3BSt2c9A4oV7Otisyws3VAZsm.jpg?quality=95&as=32x7,48x11,72x17,108x25,160x37,231x54&from=bu&cs=231x0)
![](https://sun9-6.vkuserphoto.ru/s/v1/ig2/_hyC5mbx_JGX3dO-gkeaToCUt_uGavs0J3hd-B97-FLEcUMOvjCL60lezXyV4ZEi1hbhV6j3rAMD7FDJ7Z6rekv8.jpg?quality=95&as=32x20,48x31,72x46,108x69,160x102,240x153,360x229,391x249&from=bu&cs=391x0)
![](https://sun9-71.vkuserphoto.ru/s/v1/ig2/uDCoVVamWsl-vBqtI4qkIv5hOi4Gq8pkOgfrgywwDVTwZeG6z2LRX9DTUxedx9GvQ4ERIKGQtHpEnoh-w2e4hBea.jpg?quality=95&as=32x7,48x10,72x15,108x22,160x33,232x48&from=bu&cs=232x0)
![](https://sun9-46.vkuserphoto.ru/s/v1/ig2/55PArkVjz-jD46PwkMkXNUho_GD0pUpYq-eVIb9KLTByPoSj1a_mNBVOIsihmFV7B-gO3TLU1rOe9fBc-hgcuLm2.jpg?quality=95&as=32x30,48x44,72x67,108x100,160x148,240x222,360x334,397x368&from=bu&cs=397x0)


# ЗАДАНИЕ 5.
## ТЕКСТ ЗАДАЧИ:
Реализовать 4 запроса LINQ: один с обращением к одной
таблице (перечень), один к двум таблицам (значение),
два к трём таблицам (перечень и значение).
## ЗАПРОС 5.1 — одна таблица, перечень.
Текст: перечень товаров с ценой продажи не ниже
заданной, по убыванию цены.
Алгоритм: where + orderby descending + анонимный тип;
выполнение отложенное, материализация ToList в UI.
## ТЕСТИРОВАНИЕ
Ввод: 5 → 1 → цена 9000. Ожидание: 4 строки
по убыванию: 104936 Пианино 12 000,00 руб.;
890990 Кукла Pop 810 9 800,00 руб.; 858946 Кукла
Pop 790 9 600,00 руб.; 500407 Кукла Pop 789
9 500,00 руб.; «Всего записей: 4».
Ввод: 5 → 1 → цена 20000. Ожидание:
«Всего записей: 0».

![](https://sun9-69.vkuserphoto.ru/s/v1/ig2/z8i-QoBVATTQ9bpaOPsPe9dCGbSnPtRoOrrafnYuuMcetu0RXNOc6tk0PegX_IEqDRGL3KdusKTpCT95BhWY1pIL.jpg?quality=95&as=32x32,48x48,72x72,108x109,160x161,240x241,328x330&from=bu&u=xVXV9_pd1yI3vy6y4SHQ2-hur24fKac3_N1u634WvuI&cs=328x0)
![](https://sun9-69.vkuserphoto.ru/s/v1/ig2/kkX96npryIBo8-4Lg79aFWKro-rfdBC-zrsEYo3e6YZ9LXmQA_3oILWrwf1yfzz-AqpNTcWXGRyiPpQXhtHAwPp3.jpg?quality=95&as=32x28,48x42,72x64,108x95,160x141,240x212,302x267&from=bu&cs=302x0)


## ЗАПРОС 5.2 — две таблицы, одно значение.
Текст: суммарное количество упаковок, поступивших
в магазины заданного района.
## Алгоритм: 
join движения и магазина по ID магазина,
where по типу операции и району; немедленное
выполнение Sum().
## ТЕСТИРОВАНИЕ
Ввод: 5 → 2 → район Ходунковый. Ожидание: одно
число; сверка в Excel формулой
=СУММЕСЛИ(F:F;E:E;"Поступление";C:C;"Р3")+… по
Р3, Р6, Р11, Р14 — числа совпадают.

![](https://sun9-51.vkuserphoto.ru/s/v1/ig2/y8ySZ8ZeLAcmSdPfscy1XYUkkTZEZTDTNrAyX-Grqp3VN5cNMCZAJfjtzxXooUh8my2thoim8kUmBsqBHuchgiJK.jpg?quality=95&as=32x17,48x25,72x37,108x56,160x83,240x125,360x187,480x249,540x280,541x281&from=bu&u=FXCQrDRdrca-aqWbtFJ8qbu8I42ui38KL23Cf4Ia4tE&cs=541x0)
![](https://sun9-67.vkuserphoto.ru/s/v1/ig2/ocHO43U8icK2-AfUdFLQazzU-Axe7pVAogxjK0F06zCKcXDjHPjNg-wczILc5QbzxIWAEJE4DYdzgexPxL5ZMbRc.jpg?quality=95&as=32x7,48x10,72x15,108x23,160x34,240x51,360x76,480x101,540x114,640x135,720x152,1023x216&from=bu&cs=1023x0)


## ЗАПРОС 5.3 — три таблицы, перечень.
Текст: журнал продаж за период: дата, район, товар,
количество упаковок, сумма продажи с учётом скидки
по карте.
## Алгоритм: 
два join (движение-магазин по ID,
движение-товар по артикулу), where по типу и периоду,
orderby по дате и району, анонимный тип с полем Sum
через PriceWithCard.
## ТЕСТИРОВАНИЕ
Ввод: 5 → 3 → 01.08.2024–01.08.2024. Ожидание:
154 записи (совпадает с =СЧЁТЕСЛИ в Excel);
первая строка «01.08.2024 | Колыбельный | Мягкая
игрушка Слон | 17 уп. | 32 300,00 руб.», последняя
«01.08.2024 | Юношеский | Автомобиль Gruze 750x |
13 уп. | 24 700,00 руб.»
Ввод: 5 → 3 → период 05.08.2024–01.08.2024.
Ожидание: «Ошибка: конец раньше начала.»

![](https://sun9-81.vkuserphoto.ru/s/v1/ig2/AF7QPgTUGs8qDeRscnzPX7ijvrz93330ZTG0BzN-aTK90ODsm1QUd_PvqoPcQwBAcPcTIkIolMP99uKplga8cPT9.jpg?quality=95&as=32x41,48x62,72x93,108x139,160x206,240x309,360x463,470x605&from=bu&cs=470x0)
![](https://sun9-52.vkuserphoto.ru/s/v1/ig2/X-qqZqp_5jNkZU0JXnRYYbWhUbqmws-WqqZ1btgd4X-3hY-SzNtPRA7TxPYxTGnkiH7uBLJLnoZDbnpXJHGg3LwuTNy3Wg.jpg?quality=95&as=32x26,48x38,72x58,108x86,160x128,240x192,360x288,373x298&from=bu&u=TGxQwXSsk8GgmwDIWwY8m7bakOTNKlfE0ab0YP_vRtQ&cs=373x0)
![](https://sun9-65.vkuserphoto.ru/s/v1/ig2/I7-AWDiD-p2HXJ7Jkud5u9b-sXdxxtqkzEyL1u3g9cKPMf3NmEGiOTKSndx7Q5LBw3aFk1JO5CoyY7hNBkMz0jax.jpg?quality=95&as=32x14,48x21,72x32,108x48,160x72,217x97&from=bu&cs=217x0)
![](https://sun9-56.vkuserphoto.ru/s/v1/ig2/IAseQ7jS5izVAFFxnc0BuPHx0uvB32sN0f1UoPsLIADAq7UzfyWiMYgqaBFxI00klIbzdt2LUWUbJ0XOI8QHSqTu.jpg?quality=95&as=32x6,48x9,72x14,108x21,160x31,240x46,264x51&from=bu&u=COeX0qqq_GTjiKGsWlhQL-UxsYubPKqDIGJ-dhRTUM8&cs=264x0)


## ЗАПРОС 5.4 — три таблицы, одно значение.
Текст: общая стоимость товаров заданной категории,
проданных магазинами заданного района за период
(аналог примера из задания).
## Алгоритм: 
те же join, что в 5.3, where дополнен
районом и ID категории; Sum по стоимостям строк,
скидка по карте учитывается флагом applyDiscount.
## ТЕСТИРОВАНИЕ
Ввод: 5 → 4 → Ходунковый, категория 3,
01.08.2024–05.08.2024, скидка Да. Ожидание:
848 098,00 руб. — совпадает с ответом задания
То же, скидка Нет. Ожидание: 895 800,00 руб.

![](https://sun9-87.vkuserphoto.ru/s/v1/ig2/F-vohZuGhVVuO14IPuuom7vPlDPkTAo4FiYyqrBcfyx9guXkEgB6-F7DYxgekw4qRGeX6YmqIBxclIBZ1gSc9X5d.jpg?quality=95&as=32x54,48x81,72x121,108x182,160x270,240x405,360x607,388x654&from=bu&u=qO6bB_EIJlWx1ra_RnmGebkNLWPFC3K2SHj_HisLI7A&cs=388x0)
![](https://sun9-4.vkuserphoto.ru/s/v1/ig2/rZp49ni-YZ2phoR9tac3uVBP7CjC79pBpspMKkKv8m5K8s5CQZcTFXX0yF4fX2DV5YJT5Ogk2JWaijtebuAuEptC.jpg?quality=95&as=32x47,48x70,72x106,108x158,160x235,240x352,360x528,448x657&from=bu&cs=448x0)

Формулы, которыми я пользовалась для подсчёта:
```
H2: =ВПР(D2;Товар!$A:$F;2;0)
I2: =ВПР(D2;Товар!$A:$F;5;0)
J2: =ЕСЛИ(ЕЧИСЛО(ВПР(D2;Товар!$A:$F;6;0));ВПР(D2;Товар!$A:$F;6;0);ЗНАЧЕН(ПОДСТАВИТЬ(ВПР(D2;Товар!$A:$F;6;0);"%";""))/100)
K2: =F2*I2*(1-ЕСЛИ(G2="Да";J2;0))
L2: =F2*I2
Со скидкой: =СУММПРОИЗВ((E2:E8376="Продажа")*(B2:B8376>=ДАТА(2024;8;1))*(B2:B8376<=ДАТА(2024;8;5))*(H2:H8376=3)*((C2:C8376="Р3")+(C2:C8376="Р6")+(C2:C8376="Р11")+(C2:C8376="Р14"))*K2:K8376)
Без скидки: =СУММПРОИЗВ((E2:E8376="Продажа")*(B2:B8376>=ДАТА(2024;8;1))*(B2:B8376<=ДАТА(2024;8;5))*(H2:H8376=3)*((C2:C8376="Р3")+(C2:C8376="Р6")+(C2:C8376="Р11")+(C2:C8376="Р14"))*L2:L8376)
```

![](https://sun9-48.vkuserphoto.ru/s/v1/ig2/I6XQuICWb44fSWIbQrywaLdRS-pQBNvg4SfAjY-NVt8JFLkvpxqQvxGkxFtyNNY2yL3fFp34wQSxEZXSlxnfUwFN.jpg?quality=95&as=32x14,48x21,72x31,108x46,160x69,240x103,360x155,480x206,540x232,593x255&from=bu&cs=593x0)


# ЗАДАНИЕ 6.
## ТЕКСТ ЗАДАЧИ:
сохранение изменённой базы данных
в Excel-файл.
## Алгоритм: 
по расширению создаётся HSSFWorkbook
(.xls) или XSSFWorkbook (.xlsx); обобщённый метод
WriteRows<T> пишет лист, шапку и строки через делегат
Action<IRow, T>; дата — ячейкой с форматом
dd.mm.yyyy, скидка — строкой с «%», карта — Да/Нет/-;
книга записывается в FileStream.
## ТЕСТИРОВАНИЕ
Ввод: 6 → Enter (result.xls). Ожидание: «Сохранено.»;
файл открывается в Excel, 4 листа, в «Движение
товаров» присутствует добавленная операция 9001.
Ввод: 6 → result.txt. Ожидание: «Ошибка: Нужен файл
.xls или .xlsx.»
Ввод: q. Ожидание: «До свидания!»

![](https://sun9-64.vkuserphoto.ru/s/v1/ig2/zSG8VR4NaYGSHwzptZyOX0gExRkpiqDuqwEKFJGYeqS09zlJoTTy33YZPTEHB3qKTMHotHEmfpEyzaHdxj1av4im.jpg?quality=95&as=32x21,48x32,72x48,108x71,160x106,240x159,289x191&from=bu&u=2QeLY7zp9vqNuPhn06eCLsAdpRhlv4a7Is5pQ8IiJR0&cs=289x0)
![](https://sun9-69.vkuserphoto.ru/s/v1/ig2/sHxnMuF0CVowavRmRmQxAvNjBIHPxspZ9D3fgL2Bns5GxZ4hKLC0tEVLcaJpGFyy4DB2-pr3U8oAMxBng5XBz6Wy.jpg?quality=95&as=32x19,48x28,72x43,108x64,160x95,240x142,320x190&from=bu&cs=320x0)
![](https://sun9-55.vkuserphoto.ru/s/v1/ig2/RlxTFMRxUz8ptO7MDnnyzufiemlRyBMOn4OzGvx6I4o2wP5eVD6eyiq-do7dyaT39hBT6V7J8Q_WZRCXMsDPKeSD.jpg?quality=95&as=32x3,48x4,72x6,108x9,160x13,240x20,360x30,480x40,540x45,640x54,720x60,800x67&from=bu&u=rzoPq_KQCbFOS18tDCObx1qFW2FMOrnOmBVbRcnoFt4&cs=800x0)
![](https://sun9-37.vkuserphoto.ru/s/v1/ig2/30a6aI_4-SmvJ3F439vUmRg5Zcz_MfboxpWhVU5-2BS1EbzyKWYHyFosfFIVtLHySpv1ceUtTwLdxF4HPFUDVxva.jpg?quality=95&as=32x23,48x34,72x51,108x77,160x114,240x170,252x179&from=bu&cs=252x0)

# МЕНЮ, ВЫХОД И ОБЩИЕ ПРОВЕРКИ.
меню принимает только номера заданий 1–6 или
специальное слово выхода; подменю возвращаются
по 0; при выходе с несохранёнными изменениями
программа предлагает сохранение.
## Алгоритм: 
ConsoleHelper.ReadTask читает строку,
пробует int.TryParse и сравнивает с диапазоном,
иначе сообщение и повтор; выход только по exit/q/выход;
ConfirmExit при _changes > 0 спрашивает Да/Нет и
вызывает Save.
## ТЕСТИРОВАНИЕ
Ввод в меню: abc, 0, 9, пустая строка. Ожидание:
«Ошибка: введите 1..6 или exit.» каждый раз.
Ввод в подменю: 0. Ожидание: возврат в главное меню.
Ввод: exit после добавлений. Ожидание: «Несохранённых
изменений: N.» и вопрос «Сохранить перед выходом?
(Да/Нет):».

![](https://sun9-47.vkuserphoto.ru/s/v1/ig2/iq0XAwtpUsBQQ7CRxY2ggHLCpUVmveWXhI8l-ZbwW1wxsdvOn0GrhfRZRCefHMatS7HY44EvxeO5RCvi2mzix_zN.jpg?quality=95&as=32x30,48x45,72x68,108x102,160x151,240x227,282x267&from=bu&u=e6tS-mk0ugqAc47GfseYzEq9BLxWjJWXGzk9NvtSPwk&cs=282x0)
![](https://sun9-67.vkuserphoto.ru/s/v1/ig2/371wKttEOMs7kugVrziIjPw7dWvO5B9eugfK0Dn6dRes0Mt8tvjJaFhq-_xNte_GBxLUqWNf7tOPlfKZi2IAQEAT_MuUFg.jpg?quality=95&as=32x23,48x34,72x51,108x77,160x114,240x171,288x205&from=bu&cs=288x0)
![](https://sun9-83.vkuserphoto.ru/s/v1/ig2/wBQhmwTTIhDkUklbL628JwqHVkKunY5Soj96hTHIyvnJ_TtCOgZbp_TuIDUWzKVRPbUuVeWfSTMA5eygi7tG-Bdc.jpg?quality=95&as=32x21,48x31,72x46,108x69,160x103,240x154,322x207&from=bu&cs=322x0)

# ВАЛИДАЦИЯ ВВОДА И ОБРАБОТКА ОШИБОК.
 числа через TryParse с диапазонами, запятая
принимается как точка; даты только дд.мм.гггг, период —
конец не раньше начала; ID магазина по шаблону
^Р\d{1,3}$ с заменой латинской P и регистра; артикул —
только цифры; возраст — 0+, 6+, 12+; скидка 0..100;
карта — Да/Нет; магазин, товар, категория и район —
только существующие; ключи уникальны; удаление
запрещено при зависимых записях; ошибки Excel —
с именем листа и номером строки.
## Алгоритм: 
проверки форматов полей — в ConsoleHelper,
ключей и связей — в Database, файла и листов — в
ExcelStorage; вызов любого пункта меню обёрнут в
try-catch, программа печатает текст ошибки и не
падает.
