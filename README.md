# MinimalApi для генерации задач
Простые CRUD для работы с json файлом, при взаимодействии с методом POST отправляют уведомение в Kafka, которая запущена на контейнере
Контейнер запускается с помощью файла ```docker-compose.yml```:
* Команада для запуска: ```docker-compose up -d```
* Команда для создания топика: ```docker exec minimalapipractice-kafka-1 /opt/kafka/bin/kafka-topics.sh --create --topic todo-created --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1```
* Команда для запуска приложения: ```dotnet run```

