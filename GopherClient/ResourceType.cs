using System;
using System.Collections.Generic;
using System.Text;

namespace GopherClient
{
	public enum ResourceType
	{
		Unknown,
		Gopher,
		Text,
		Image
	}

	public static class ResourceTypeMap
    {
		public static ResourceType GetResourceType(char identifier)
        {	
            switch (identifier)
            {
				case '0':
					return ResourceType.Text;
				case '1':
					return ResourceType.Gopher;
				case '9'://binary
					return ResourceType.Text;
				case 'g'://gif
					return ResourceType.Image;
				case 'I'://image
					return ResourceType.Image;
				case '.':
					return ResourceType.Unknown;
				case ' ':
					return ResourceType.Unknown;
			}

			throw new KeyNotFoundException($"'{identifier}' is Not defined!");
        }

		public static char GetResourceIdentifier(ResourceType type)
		{
			switch (type)
			{
				case ResourceType.Text:
					return '0';
				case ResourceType.Gopher:
					return '1';
				case ResourceType.Image:
					return 'I';
				default:
					return '.';
			}
		}
	}
}