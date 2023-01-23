using System.Text;

Console.Error.WriteLine("args: [{0}]", string.Join(", ", args));

var dict = new Dictionary<string, IList<string>>(StringComparer.OrdinalIgnoreCase);
string? line;
while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
{
	Console.Error.WriteLine("stdin: {0}", line);

	string[] kvp = line.Split('=', 2, StringSplitOptions.None);
	string key = kvp[0];
	string value = kvp[1];

	if (!dict.TryGetValue(key, out IList<string>? values))
	{
		values = new List<string>();
		dict[key] = values;
	}

	bool isMulti = key.EndsWith("[]");
	if (isMulti)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			values.Clear();
			continue;
		}
	}
	else
	{
		values.Clear();
	}

	values.Add(value);
}

if (!StringComparer.OrdinalIgnoreCase.Equals(args[0], "get"))
{
	return;
}

using var tty = new FileStream("/dev/tty", FileMode.Open, FileAccess.ReadWrite);

do
{
	Write(tty, "stdout: ");
	line = ReadLine(tty);
	Console.WriteLine(line);
} while (!string.IsNullOrWhiteSpace(line));

void Write(FileStream fs, string text)
{
	byte[] bytes = Encoding.UTF8.GetBytes(text);
	fs.Write(bytes, 0, bytes.Length);
}

string ReadLine(FileStream fs)
{
	var bytes = new List<byte>();
	int i;
	while ((i = fs.ReadByte()) > 0 && i != '\n')
	{
		bytes.Add((byte)i);
	}

	if (bytes.Count > 0 && bytes[^1] == '\r')
	{
		bytes.RemoveAt(bytes.Count - 1);
	}

	return Encoding.UTF8.GetString(bytes.ToArray());
}
