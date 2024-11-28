import 'dart:math';
import 'package:xml/xml.dart';

class Bible {
  final List<Book> books;

  Bible({required this.books});

  factory Bible.fromXml(String xmlString) {
    final document = XmlDocument.parse(xmlString);
    final books = document.findAllElements('book').map((bookElement) {
      return Book.fromXml(bookElement);
    }).toList();

    return Bible(books: books);
  }

  Book getRandomBook() {
    final randomIndex = Random().nextInt(books.length);
    return books[randomIndex];
  }
}

class Book {
  final String name;
  final List<Chapter> chapters;

  Book({required this.name, required this.chapters});

  factory Book.fromXml(XmlElement bookElement) {
    final name = bookElement.getAttribute('name') ?? '';
    final chapters = bookElement.findElements('c').map((chapterElement) {
      return Chapter.fromXml(chapterElement);
    }).toList();

    return Book(name: name, chapters: chapters);
  }

  Chapter getRandomChapter() {
    final randomIndex = Random().nextInt(chapters.length);
    return chapters[randomIndex];
  }
}

class Chapter {
  final int number;
  final List<Verse> verses;

  Chapter({required this.number, required this.verses});

  factory Chapter.fromXml(XmlElement chapterElement) {
    final number = int.parse(chapterElement.getAttribute('n') ?? '0');
    final verses = chapterElement.findElements('v').map((verseElement) {
      return Verse.fromXml(verseElement);
    }).toList();

    return Chapter(number: number, verses: verses);
  }

  Verse getRandomVerse() {
    final randomIndex = Random().nextInt(verses.length);
    return verses[randomIndex];
  }
}

class Verse {
  final int number;
  final String text;

  Verse({required this.number, required this.text});

  factory Verse.fromXml(XmlElement verseElement) {
    final number = int.parse(verseElement.getAttribute('n') ?? '0');
    final text = verseElement.text;

    return Verse(number: number, text: text);
  }
}
