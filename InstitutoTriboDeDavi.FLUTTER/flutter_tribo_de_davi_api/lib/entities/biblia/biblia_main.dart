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

  // Função para pesquisar um livro pelo nome
  Book? findBookByName(String name) {
    return books.firstWhere((book) => book.name == name);
  }

  // Função para buscar versículo pelo livro, capítulo e versículo
  String? findVerse(String bookName, int chapterNumber, int verseNumber) {
    final book = findBookByName(bookName);
    if (book != null) {
      final chapter = book.findChapterByNumber(chapterNumber);
      if (chapter != null) {
        final verse = chapter.findVerseByNumber(verseNumber);
        return verse?.text;
      }
    }
    return null;
  }
}

class Book {
  final String name;
  final String abbrev;
  final int chaptersCount;
  final List<Chapter> chapters;

  Book({
    required this.name,
    required this.abbrev,
    required this.chaptersCount,
    required this.chapters,
  });

  factory Book.fromXml(XmlElement bookElement) {
    final name = bookElement.getAttribute('name') ?? '';
    final abbrev = bookElement.getAttribute('abbrev') ?? '';
    final chaptersCount =
        int.parse(bookElement.getAttribute('chapters') ?? '0');

    final chapters = bookElement.findElements('c').map((chapterElement) {
      return Chapter.fromXml(chapterElement);
    }).toList();

    return Book(
      name: name,
      abbrev: abbrev,
      chaptersCount: chaptersCount,
      chapters: chapters,
    );
  }

  // Função para buscar um capítulo pelo número
  Chapter? findChapterByNumber(int number) {
    return chapters.firstWhere((chapter) => chapter.number == number);
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

  // Função para buscar um versículo pelo número
  Verse? findVerseByNumber(int number) {
    return verses.firstWhere((verse) => verse.number == number);
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
