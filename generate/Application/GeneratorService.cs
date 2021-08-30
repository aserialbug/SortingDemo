using generate.Infrastructure;

namespace generate.Application
{
    public class GeneratorService
    {
        private readonly LineBuilder _lineBuilder;
        private readonly OutputContainer _outputContainer;

        public GeneratorService(LineBuilder lineBuilder, OutputContainer outputContainer)
        {
            _outputContainer = outputContainer;
            _lineBuilder = lineBuilder;
        }

        public void Generate(long outputFileSizeBytes)
        {
            while (true)
            {
                // Создаем строку
                var currentLine = _lineBuilder.BuildLine();
                
                // Следим чтобы файл был не больше чем outputFileSizeBytes байт
                if(_outputContainer.Length + currentLine.Length > outputFileSizeBytes)
                    break;
                
                // Добавляем строку в файл
                _outputContainer.AppendLine(currentLine);
            }
        }
    }
}