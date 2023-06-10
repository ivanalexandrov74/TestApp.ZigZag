using ZigZag.Test.Dto;

namespace ZigZag.Test.Data
{
    public class GraphQlExternalApiResponseDto
    {

        public DataDto data = new();


        public class DataDto
        {
            public externalApisDto externalApis = new();
        }
        public class externalApisDto
        {
            public List<ExternalApiNodeDto> nodes = new();

            public PageInfoGraphQlDto pageInfo = new();
        }


    }
}
