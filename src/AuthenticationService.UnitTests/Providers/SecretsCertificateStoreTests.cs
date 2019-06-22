using AuthenticationService;
using AuthenticationService.Providers;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AuthenticationService.UnitTests.Providers
{
    public class SecretsCertificateStoreTests
    {
        private readonly Mock<ISecretsStore> mockSecretsStore;
        private readonly ICertificateStore certificateStore;

        public SecretsCertificateStoreTests()
        {
            this.mockSecretsStore = new Mock<ISecretsStore>();
            this.certificateStore = new SecretsCertificateStore(this.mockSecretsStore.Object);
        }

        [Fact]
        public async void MustGetCertificate()
        {
            // cat <PKCS12 certificate path> | gzip | base64 -w 0
            const string payload = "H4sIADA9DV0AA1XUZzQbChsHcIkEQVEa1YjamzRiq2v31mpwbUXFjtVaoagRuzVKUWKPojat2dLaNIpWVaT2Su09qqqve8677ofnPOf5/8/z9YfE018GAqiReMgEDUQsRjvm+BKAFlCAhwyA8JCei/g9Eg8K+F9FU4AHuV5EjkAA1cWy+b8nJJyG/t8HI4AGyQSi65PPtf6gLHoFCKSjCseDkBxnY+tOYthRSHtEOUtzp9n9b8etntNQiphd/4H2i3dQ7+U89lYf3ZbyjiGHBPURkZfeYr8G3h1+gHcb3GIWS7WZZKaI9h43SBdK1XrUyXK3V5CPQ9lPXN4J1OupG81ju91Dj7STnKtUTwwhIr3KXP3D0fSfYp0FnV3ztvtQ5yvDh9DW5gq1isK+qUaHnNRXcE6QDZwm0y5ZOQcxPluQafti5tl2btJq2RmCewS+ZTuI2iv4/DaFravdClsUWvc+dM5aOl7ehyrAQM/dG9xOb6znmaJmFn19M99DA+srnZ8JOd4l4nXaLARdF4GCURVTHG65zcQ1nF/1eIxM4VWrs/rLM4QV8fCby2d8Yi+hb+7puss1GMOW+ftvtZY8PbUvPr0VF3VOorZU1m9pJciQ3JLD8xg8XoBL+LyG2Gnq1YXodIkycO13OJ4T+oNDhN/J3oLk0jqIXLfdGa/CHpem3knenAkh6KxgPxHSmmgkDF3pXSJi45ycWiDExJ5YUZLmRqMI4oxiIz51KAvqw/EMjeVnwfZn7BRj3r4gx2X7s85qxqvpBoI7vbCPZCQUmfufaj6yFbdTyx2MLbNnSB5RZZaTQSsLjmMt/RcJwx1xWnTIy+SsoIFEzJA7kM2gSaQYW4OIjCiJfJNIEo6hRQmvyyZ2QjNo5Cq47rla+rKFZTrTdCgBB1O3oZa4CH7WoRGjTTHQvZ8H3IGVnfcGTDKV0uhUeK0HVBjyYKux+UbdT/TF1DDg60cCC3c3MrKGaoi0ERRDzqfcnLk3VAQywpKD74g5D89w9mQZp5R/hP/5XBGwtPW25eGPoxXSYkgCfkOTtcySjc61Rs+qGryLb56kpfLs0it111z5InwndPHrCG8WO8XhV0frh26ADNR94aZ5RO3XIOhHebZwe5xf6SDcYfFgoXFnjguDXBIJ3dOw5B9cUPd8bx0ULO+SLn9dMwlKytJvHDk63+vtqTJmKUVF1XB8d5R8NtpcnDmNFv4xxTOjGz92HFm9uEvbNOUYnVrXt2FfVa7wJ9sv2JqhL6ETENfu2Lq9BhgvWokE7ZSf1a0F1tX4TDuEZ6NrO8yG7zaquDMT+yuM4H7+2c1CEzEspR6S1ogTwAxvihVez3A6GDwwOKiokvFY2ISPsd7QIDpycV87+5XJbWrEk50NnrQHnL/Rn5XY7ZCZPRhdCAxnhU6PCJCdlCiPiLFlOEQOEXB8fHPNa8xH5/cT5cS1/7h+ErcGCaVDeEJpH6zLmQsGjauh4PuKtnznb6Ypbuc2vmFqDlRGuzV/Hf3WM5Sg6AhO63yZ3/vqczjgLvAZnPJVqXxf4M4b8hbza1/DQ0TO5HYXSfHXk7SBvvuGXZSSoLT50GeZgzGvtSZtwC4LTr67V1h3F7CJTwxuBR3I3x0wOy8uqfQbO0xdnguW4vwrkoLEg9X/oRIYBcKDERex2MUI0TD8hxt6APDCpa0LkNb+oRD13wqt1LpZOUnKhvytEAgPGhCC5ZbasGblDs6bnV9PmMMZgFO7t1WbRSWLsY/OHUGSmOFlwturapm96LKYayEjEuMrY+LPMxg/tO/5LT015ytajvoZPTaR2MmnSFS8/TyoJK4E/uzH2HAh54hCz7kBs52JqZ/3bS+lNC+rqHmq7VqTlCzviEYW79eJAwFgbzletyiUbwtB3a1rrYBEIu9+L8QxHCql5TaMwk+QZZW1HAb7Fu2nZcmqpXyPEC3BAbCSs6XMRg4XCfTSK5zjET3QL6E7JK9tBgb/bBG7a53xJAlW33JD1Sh7DVDQY0I36IULDB5GS2VsRLW+xPkjaE2KfjqadzDmVvO4qx9YIgqt207ZeToMT43mZfR03FVNA1muhRtrnS5ry6Al05MCXBJyrkUQdkxnjIZNHQt3bs9EBwXFkp9pJJE2LrN8JFaifsbvuWRLyqIfjS1BV59qOymcFKIeAycvdw9vDmAotatLdzHUHD478B/l1MnT+nedpcWwTme/zVbCYFC2LVYOC6qiu87aGwlgSblHelPPgx9AfAEw8zCu4ABre8Hi32rRBp/tT1L8qzZTs5z3luqbXkqnjxKDJzfdft/WNeH6OHlr7nhowSrr0zXxMKTmOHJCvL1BXn6GzNtW8qkubBbpV95lyaQvwfjzSbOzjimiLaFKWpgyppByRjzbW19NyMG4FvjM4wpy+82iZT9lF2pbpEVSlf/If1GM7qTn15x8TvGod52r4XLs6TkRcBFkm9XgT+pVulFwrFJAedomOiW1i1kHqK4HoR7yg37+YiNEN1m4rbVleGr1G2R8teOiTai1eyxTuNXHZAVZFOv18c3jyLYI9qBZrz6pIl4bynQI7EOFfM/jf75juipxhYxpTxYV4Qo5VYr4fo+KUiD46+HLojp5GxbBS60t8lAO3zF64uUVG52DKB2uJq8YAWnzssGeqvu1wLQQgUrO3yonyBGQ28KCzVr+vMuduNmW5qtxmn+IxnglDaVIqMtt48/SJcysTRN7FMiFjSSPp8AN+h6hduMPKz42iz1nHo5X9p1tFeleKX1s0LcrSsQx7anuC7O9M0v4WPwNn0Wiw+hwwJuA/rptCzlZJo9OD2ckQVKtuTe4k58JkfskDntboPGXSk1VvhQpqA2izadE7r/2AcgY2PPqzdgop1e6EfnV3qUzFF+tiPE5YI0Kj04TSdVCcygvINcYUcbuuEW671oHpopLStGrrJ7kt1qhkoU//E1GeyG1LbQr/j6u3V0ifeiBtBwabIGh3rWdes6JB7Keuc7U3oaw2t3ADlOBZPimFqaXe4uaBFXT95jkqK7NuZ3vRxJboUhCDzbrgBHWQq8+N8+/eLgQgMN+Oioj76M2UyeFNgM6BKIEA3JG17UncR06Rq+EuSv8Z8oXCcHIl2gDokWRoCORKWjktU4EeRbR0uukdPgNbKys3DL10LLIONq2ujSg4bSH9t18XsEcFWBaI+m1ywd0TPh6+WxDlgKgBReY25aOpU2BM4nRM9PT/aGRGEecdSfIfItXdEWZbL03cYENGeJlCmsXfZ7zVnv4FE0kXv3GpmV8A5P4eIKpmBNz80vDRGM+4dLRtzh+jC3k3hDHTWp0mJKjd+Erv+DvfrBKue2iujiePxUNH15KkRJE8v9XRwhUih105f5aqodukIbr8TpdP9sQY8ZXfAAJKYXkRUJowOJM1EAYmAp05ap0PmFavMkxCFsbjOW1G1ViYA4D0QGnBFG+cQJCfyv5L6HZYBkVCgAA";
            const string name = "test-certificate";  
            var guid = Guid.NewGuid().ToByteArray();

            mockSecretsStore.Setup(x => x.GetSecret(name)).Returns(Task.FromResult(payload));
            
            var certificate = await certificateStore.GetCertificateAsync(name, "H34ea[ptgejg");

            Assert.NotNull(certificate);
        }
    }
}