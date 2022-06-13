using System;
using System.Collections.Generic;
using System.Text;

namespace GopherClient
{
	public enum GopherResourceType
	{
		Unknown,
		Gopher,
		Text,
		Image
	}

	public static class ResourceTypeMap
    {
		public static GopherResourceType GetResourceType(char identifier)
        {	
            switch (identifier)
            {
				case '0':
					return GopherResourceType.Text;
				case '1':
					return GopherResourceType.Gopher;
				case '9'://binary
					return GopherResourceType.Text;
				case 'g'://gif
					return GopherResourceType.Image;
				case 'I'://image
					return GopherResourceType.Image;
				case 'h':
					return GopherResourceType.Unknown;
				case '.':
					return GopherResourceType.Unknown;
				case ' ':
					return GopherResourceType.Unknown;
				default:
					return GopherResourceType.Unknown;
			}
        }

		public static char GetResourceIdentifier(GopherResourceType type)
		{
			switch (type)
			{
				case GopherResourceType.Text:
					return '0';
				case GopherResourceType.Gopher:
					return '1';
				case GopherResourceType.Image:
					return 'I';
				default:
					return '.';
			}
		}
	}
}